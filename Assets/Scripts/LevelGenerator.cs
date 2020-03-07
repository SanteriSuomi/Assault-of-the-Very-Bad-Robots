using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AOTVBR
{
    public class LevelGenerator : Singleton<LevelGenerator>
    {
        public delegate void GeneratingTextHide();
        public event GeneratingTextHide GeneratingTextHideEvent;

        [SerializeField]
        private GameObject levelPrefab = default;
        [SerializeField]
        private Transform levelPrefabParent = default;
        [SerializeField]
        private GameObject levelPathPrefab = default;
        [SerializeField]
        private GameObject cameraPivot = default;
        [SerializeField]
        private NavMeshSurface navMesh = default;
        [SerializeField]
        private GameObject basePrefab = default;
        [SerializeField]
        private GameObject shipPrefab = default;

        [SerializeField]
        private Quaternion enemyShipBaseStartRotation = Quaternion.Euler(0, 270, 0);
        [SerializeField]
        private Vector3 enemyShipBaseStartPositionOffset = new Vector3(0.08f, 0, -2.15f);

        private GameObject enemyShipBase;
        private GameObject playerBase;
        private GameObject[,,] map;

        [SerializeField]
        private int xLength = 24;
        [SerializeField]
        private int zLength = 22;
        [Tooltip("One Y row is every 0.5.")]
        [SerializeField]
        private float yLength = 0.5f;
        public float YLength { get => yLength; }
        [SerializeField]
        private float yInitializationOffset = 0.5f;

        [SerializeField]
        private int walkableLayerInt = 9;
        [SerializeField]
        private int nonWalkableLevelLayerInt = 11;

        private bool hasSetAgentStartPoint;

        protected override void Awake()
            => map = new GameObject[xLength, Mathf.RoundToInt(yLength + yLength), zLength];

        private void OnEnable() => InitializeCamera();

        private void InitializeCamera()
            => Instantiate(cameraPivot).transform.position = new Vector3(xLength / 2, Mathf.RoundToInt(yLength + yLength), zLength / 2);

        private void Start() => GenerateMap();

        public void GenerateMap() => StartCoroutine(GenerateMapTimer());

        private IEnumerator GenerateMapTimer()
        {
            yield return null;
            ClearCurrentMap();
            InitializeRandomSeed();
            GenerateMapBlocks();
            GenerateMapPath();
            GenerateNavMesh();
            ForceCleanUp();
            GeneratingTextHideEvent.Invoke();
        }

        #region Generate Map
        private void GenerateMapBlocks()
        {
            try
            {
                for (int x = 0; x < xLength; x++)
                {
                    InitializeX(x);
                    for (int z = 0; z < zLength; z++)
                    {
                        InitializeZ(x, z);
                        for (float y = 0; y < yLength; y += yInitializationOffset)
                        {
                            InitializeY(x, z, y);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                // Ignore exceptions
            }
        }

        private void InitializeX(int x)
        {
            GameObject xObj = Instantiate(levelPathPrefab);
            xObj.transform.SetParent(levelPrefabParent);
            xObj.layer = walkableLayerInt;
            xObj.transform.position = new Vector3(x, 0, 0);
            map[x, 0, 0] = xObj;
        }

        private void InitializeZ(int x, int z)
        {
            GameObject zObj = Instantiate(levelPathPrefab);
            zObj.transform.SetParent(levelPrefabParent);
            zObj.layer = walkableLayerInt;
            zObj.transform.position = new Vector3(x, 0, z);
            map[x, 0, z] = zObj;
        }

        private void InitializeY(int x, int z, float y)
        {
            GameObject yObj = Instantiate(levelPrefab);
            yObj.transform.SetParent(levelPrefabParent);
            yObj.layer = nonWalkableLevelLayerInt;
            yObj.AddComponent<NavMeshObstacle>().carving = true; // Level blocks that are not the path should not have navmesh.
            yObj.transform.position = new Vector3(x, y + yInitializationOffset, z);
            map[x, Mathf.RoundToInt(y + y), z] = yObj;
        }
        #endregion

        #region Generate Path
        private int thirdPathXStartPos;
        private int fourthPathZCarveAmount;
        private int fifthPathXStartPos;

        private void GenerateMapPath()
        {
            try
            {
                GenerateFirstPath(out int firstPathXStartPos, out int firstPathZCarveAmount);

                SpawnShip();
                hasSetAgentStartPoint = false;// Indicate that agentStartPoint can be set again.

                GenerateSecondPath(firstPathXStartPos, firstPathZCarveAmount);
                GenerateThirdPath(firstPathZCarveAmount);
                GenerateFourthPath();

                Vector3 agentEndPoint = GenerateFifthPath();

                SpawnBaseAt(agentEndPoint);
                LevelData.Instance.AgentEndPoint = agentEndPoint;
            }
            catch (System.Exception)
            {
                // If there is a problem with the generation, start a new one.
                GenerateMap();
            }
        }

        private void GenerateFirstPath(out int firstPathXStartPos, out int firstPathZCarveAmount)
        {
            firstPathXStartPos = Random.Range(2, xLength - 2);
            SetAgentStart(firstPathXStartPos);
            
            firstPathZCarveAmount = Random.Range(2, zLength / 2);
            for (int i = 0; i < firstPathZCarveAmount; i++)
            {
                Destroy(map[firstPathXStartPos, 0, i]);
            }
        }

        private void GenerateSecondPath(int firstPathXStartPos, int firstPathZCarveAmount)
        {
            int secondPathXStartPos = Random.Range(2, xLength - 2);
            int secondPathCarveDirection = Random.Range(0, 2);
            for (int i = 0; i < secondPathXStartPos; i++)
            {
                thirdPathXStartPos = 0;
                if (secondPathCarveDirection == 0)
                {
                    thirdPathXStartPos = firstPathXStartPos + i;
                    Destroy(map[firstPathXStartPos + i, 0, firstPathZCarveAmount]);
                }
                else
                {
                    thirdPathXStartPos = firstPathXStartPos - i;
                    Destroy(map[firstPathXStartPos - i, 0, firstPathZCarveAmount]);
                }
            }
        }

        private void GenerateThirdPath(int firstPathZCarveAmount)
        {
            int thirdPathZCarveAmount = Random.Range(2, zLength / 2);
            for (int i = 0; i < thirdPathZCarveAmount; i++)
            {
                fourthPathZCarveAmount = 0;
                fourthPathZCarveAmount = firstPathZCarveAmount + i;
                Destroy(map[thirdPathXStartPos, 0, firstPathZCarveAmount + i]);
            }
        }

        private void GenerateFourthPath()
        {
            int fourthPathXStartPos = Random.Range(2, xLength - 2);
            int fourthPathDirection = Random.Range(0, 2);
            for (int i = 0; i < fourthPathXStartPos; i++)
            {
                fifthPathXStartPos = 0;
                if (fourthPathDirection == 0)
                {
                    fifthPathXStartPos = thirdPathXStartPos + i;
                    Destroy(map[thirdPathXStartPos + i, 0, fourthPathZCarveAmount]);
                }
                else
                {
                    fifthPathXStartPos = thirdPathXStartPos - i;
                    Destroy(map[thirdPathXStartPos - i, 0, fourthPathZCarveAmount]);
                }
            }
        }

        private Vector3 GenerateFifthPath() // Last path
        {
            // Make sure to generate the path to the end of the map.
            int fifthPassAmountZ = zLength - fourthPathZCarveAmount;
            Vector3 agentEndPoint = Vector3.zero;
            for (int i = 0; i < fifthPassAmountZ; i++)
            {
                Destroy(map[fifthPathXStartPos, 0, fourthPathZCarveAmount + i]);
            }

            for (int i = 0; i < fifthPassAmountZ; i++)
            {
                agentEndPoint = SetAgentEnd(agentEndPoint, i);
            }

            return agentEndPoint;
        }

        private void SpawnShip()
        {
            if (shipPrefab != null)
            {
                Destroy(enemyShipBase);
            }

            enemyShipBase = Instantiate(shipPrefab);
            SetShipTransform();

            void SetShipTransform()
            {
                enemyShipBase.transform.rotation = enemyShipBaseStartRotation;
                enemyShipBase.transform.position =
                    LevelData.Instance.AgentStartPoint + enemyShipBaseStartPositionOffset;
            }
        }

        private void SpawnBaseAt(Vector3 agentEndPoint)
        {
            if (playerBase != null)
            {
                Destroy(playerBase);
            }

            playerBase = Instantiate(basePrefab);
            playerBase.transform.rotation = Quaternion.Euler(0, 180, 0);
            playerBase.transform.position = agentEndPoint + new Vector3(0, 0, 3.695f);
        }
        #endregion

        #region Other Map Methods
        private void ClearCurrentMap()
        {
            // When generating a new map, destroy all the blocks currently existing.
            foreach (GameObject block in map)
            {
                Destroy(block);
            }
        }

        private void InitializeRandomSeed()
        {
            // Initialize the random seed with a value from system time.
            Random.InitState((int)System.DateTime.Now.Ticks);
        }

        private void GenerateNavMesh()
        {
            // Build the agent navmesh.
            navMesh.BuildNavMesh();
        }

        private void ForceCleanUp()
        {
            // Clean all the garbage before starting a game to prevent accidental hitches.
            System.GC.Collect();
            // Make sure there is no unused resources loaded.
            Resources.UnloadUnusedAssets();
        }

        private void SetAgentStart(int firstPathXStartPos)
        {
            if (!hasSetAgentStartPoint)
            {
                hasSetAgentStartPoint = true;
                LevelData.Instance.AgentStartPoint = map[firstPathXStartPos, 0, 0].transform.position;
            }
        }

        private Vector3 SetAgentEnd(Vector3 agentEndPoint, int i)
        {
            if (map[fifthPathXStartPos, 0, fourthPathZCarveAmount + i].transform.position.z > agentEndPoint.z)
            {
                agentEndPoint = map[fifthPathXStartPos, 0, fourthPathZCarveAmount + i].transform.position;
            }

            return agentEndPoint;
        }
        #endregion
    }
}