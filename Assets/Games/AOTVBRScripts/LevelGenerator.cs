using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AOTVBR
{
#pragma warning disable S1215 // "GC.Collect" should not be called
    public class LevelGenerator : Singleton<LevelGenerator>
    {
        public delegate void GeneratingTextHide();
        public event GeneratingTextHide GeneratingTextHideEvent;

        [SerializeField]
        private GameObject levelPrefab = default;
        [SerializeField]
        private GameObject levelPathPrefab = default;

        private GameObject enemyShipBase;
        private GameObject playerBase;
        private GameObject[,,] map;
        [SerializeField]
        private MeshFilter pathMeshFilter = default;
        [SerializeField]
        private MeshFilter levelMeshFilter = default;
        private MeshRenderer pathMeshRenderer;
        private MeshRenderer levelMeshRenderer;
        [SerializeField]
        private Material pathMaterial = default;
        [SerializeField]
        private Material levelMaterial = default;

        [SerializeField]
        private Transform levelPrefabParent = default;
        [SerializeField]
        private GameObject basePrefab = default;
        [SerializeField]
        private GameObject shipPrefab = default;
<<<<<<< HEAD:Assets/Scripts/LevelGenerator.cs

        [SerializeField]
        private Quaternion shipStartRotation = Quaternion.Euler(0, 270, 0);
        [SerializeField]
        private Vector3 shipStartPositionOffset = new Vector3(0.08f, 0, -2.15f);
        [SerializeField]
        private Quaternion baseStartRotation = Quaternion.Euler(0, 180, 0);
        [SerializeField]
        private Vector3 baseStartPositionOffset = new Vector3(0, 0, 3.695f);
=======
        [SerializeField]
        private NavMeshSurface navMesh = default;
        [SerializeField]
        private GameObject cameraPivot = default;

        private GameObject enemyShipBase;
        private GameObject playerBase;
        private GameObject[,,] map;
>>>>>>> bugfixes:Assets/Games/AOTVBRScripts/LevelGenerator.cs

        [SerializeField]
        private Quaternion shipStartRotation = Quaternion.Euler(0, 270, 0);
        [SerializeField]
        private Vector3 shipStartPositionOffset = new Vector3(0.08f, 0, -2.15f);
        [SerializeField]
        private Quaternion baseStartPosition = Quaternion.Euler(0, 180, 0);
        [SerializeField]
        private Vector3 baseStartPositionOffset = new Vector3(0, 0, 3.695f);

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
        private int nonWalkableLayerInt = 11;

        protected override void Awake()
        {
            map = new GameObject[xLength, Mathf.RoundToInt(yLength + yLength), zLength];
            pathMeshRenderer = pathMeshFilter.GetComponent<MeshRenderer>();
            levelMeshRenderer = levelMeshFilter.GetComponent<MeshRenderer>();
        }

        private void OnEnable() => InitializeCamera();

        private void InitializeCamera()
            => Instantiate(cameraPivot).transform.position = new Vector3(
                xLength / 2,
                Mathf.RoundToInt(yLength + yLength),
                zLength / 2);

        private void Start() => GenerateMap();

        public void GenerateMap() => StartCoroutine(GenerateMapCoroutine());

        private IEnumerator GenerateMapCoroutine()
        {
            yield return null;
            ClearCurrentMap();
            InitializeRandomSeed();
            GenerateMapBlocks();
            GenerateMapPath();
            GenerateNavMesh();
<<<<<<< HEAD:Assets/Scripts/LevelGenerator.cs
            CombineMap();
=======
>>>>>>> bugfixes:Assets/Games/AOTVBRScripts/LevelGenerator.cs
            ForceCleanUp();
            GeneratingTextHideEvent.Invoke();
        }

        #region Generate Map
        private void GenerateMapBlocks()
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

        private void InitializeX(int x)
        {
            GameObject xObj = Instantiate(levelPathPrefab);
            xObj.layer = walkableLayerInt;
            xObj.isStatic = true;
            xObj.transform.position = new Vector3(x, 0, 0);
            map[x, 0, 0] = xObj;
        }

        private void InitializeZ(int x, int z)
        {
            GameObject zObj = Instantiate(levelPathPrefab);
            zObj.layer = walkableLayerInt;
            zObj.isStatic = true;
            zObj.transform.position = new Vector3(x, 0, z);
            map[x, 0, z] = zObj;
        }

        private void InitializeY(int x, int z, float y)
        {
            GameObject yObj = Instantiate(levelPrefab);
<<<<<<< HEAD:Assets/Scripts/LevelGenerator.cs
            yObj.layer = nonWalkableLayerInt;
            //yObj.AddComponent<NavMeshObstacle>().carving = true; // Level blocks that are not the path should not have navmesh.
=======
            yObj.transform.SetParent(levelPrefabParent);
            yObj.layer = nonWalkableLevelLayerInt;
            yObj.isStatic = true;
            yObj.AddComponent<NavMeshObstacle>().carving = true; // Level blocks that are not the path should not have navmesh.
>>>>>>> bugfixes:Assets/Games/AOTVBRScripts/LevelGenerator.cs
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
                SetAgentStart(firstPathXStartPos);
                SpawnShip();
                GenerateSecondPath(firstPathXStartPos, firstPathZCarveAmount);
                GenerateThirdPath(firstPathZCarveAmount);
                GenerateFourthPath();
                int fifthPassAmountZ = GenerateFifthPath();
                LevelData.Instance.AgentEndPoint = GetAgentEnd(fifthPassAmountZ);
                SpawnBase();
            }
            catch (IndexOutOfRangeException e)
            {
#if UNITY_EDITOR
                Debug.Log(e);
#endif

                // If there is a problem with the generation, start a new one.
                GenerateMap();
            }
        }

        private void GenerateFirstPath(out int firstPathXStartPos, out int firstPathZCarveAmount)
        {
            firstPathXStartPos = Random.Range(2, xLength - 2);
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

        private int GenerateFifthPath()
        {
            // Make sure to generate the path to the end of the map.
            int fifthPathRemainingZAmount = zLength - fourthPathZCarveAmount;
            for (int i = 0; i < fifthPathRemainingZAmount; i++)
            {
                Destroy(map[fifthPathXStartPos, 0, fourthPathZCarveAmount + i]);
            }

            return fifthPathRemainingZAmount;
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
                enemyShipBase.transform.rotation = shipStartRotation;
                enemyShipBase.transform.position =
                    LevelData.Instance.AgentStartPoint + shipStartPositionOffset;
            }
        }

        private void SpawnBase()
        {
            if (playerBase != null)
            {
                Destroy(playerBase);
            }

            playerBase = Instantiate(basePrefab);
            SetBaseTransform();

            void SetBaseTransform()
            {
<<<<<<< HEAD:Assets/Scripts/LevelGenerator.cs
                playerBase.transform.rotation = baseStartRotation;
                playerBase.transform.position =
=======
                playerBase.transform.rotation = baseStartPosition;
                playerBase.transform.position = 
>>>>>>> bugfixes:Assets/Games/AOTVBRScripts/LevelGenerator.cs
                    LevelData.Instance.AgentEndPoint + baseStartPositionOffset;
            }
        }

        private void SetAgentStart(int firstPathXStartPos) 
            => LevelData.Instance.AgentStartPoint = map[firstPathXStartPos, 0, 0].transform.position;

        private Vector3 GetAgentEnd(int fifthPathZCarveAmount)
        {
            Vector3 agentEndPoint = Vector3.zero;
            for (int i = 0; i < fifthPathZCarveAmount; i++)
            {
                if (map[fifthPathXStartPos, 0, fourthPathZCarveAmount + i].transform.position.z > agentEndPoint.z)
                {
                    agentEndPoint = map[fifthPathXStartPos, 0, fourthPathZCarveAmount + i].transform.position;
                }
            }

            return agentEndPoint;
        }
        #endregion

        #region Other Map Methods
        private void ClearCurrentMap()
        {
<<<<<<< HEAD:Assets/Scripts/LevelGenerator.cs
            //foreach (GameObject block in map)
            //{
            //    Destroy(block);
            //}
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    for (int z = 0; z < map.GetLength(2); z++)
                    {
                        Debug.Log("asd");
                        Destroy(map[x, y, z]);
                    }
=======
            GameObject[] sceneObjects = FindObjectsOfType<GameObject>();
            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if (sceneObjects[i].layer == walkableLayerInt
                    || sceneObjects[i].layer == nonWalkableLevelLayerInt)
                {
                    Destroy(sceneObjects[i]);
>>>>>>> bugfixes:Assets/Games/AOTVBRScripts/LevelGenerator.cs
                }
            }
        }

        private void InitializeRandomSeed()
            => Random.InitState((int)DateTime.Now.Ticks);

        private void GenerateNavMesh()
            => navMesh.BuildNavMesh();

        private void ForceCleanUp()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }
<<<<<<< HEAD:Assets/Scripts/LevelGenerator.cs

        private void SetAgentStart(int firstPathXStartPos)
            => LevelData.Instance.AgentStartPoint = map[firstPathXStartPos, 0, 0].transform.position;

        private Vector3 GetAgentEnd(int fifthPathZCarveAmount)
        {
            Vector3 agentEndPoint = Vector3.zero;
            for (int i = 0; i < fifthPathZCarveAmount; i++)
            {
                if (map[fifthPathXStartPos, 0, fourthPathZCarveAmount + i].transform.position.z > agentEndPoint.z)
                {
                    agentEndPoint = map[fifthPathXStartPos, 0, fourthPathZCarveAmount + i].transform.position;
                }
            }

            return agentEndPoint;
        }

        private void CombineMap()
        {
            List<MeshFilter> pathMeshFilters = new List<MeshFilter>();
            List<MeshFilter> levelMeshFilters = new List<MeshFilter>();
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    for (int z = 0; z < map.GetLength(2); z++)
                    {
                        if (map[x,y,z].layer == walkableLayerInt)
                        {
                            pathMeshFilters.Add(map[x,y,z].GetComponent<MeshFilter>());
                        }
                        else
                        {
                            levelMeshFilters.Add(map[x,y,z].GetComponent<MeshFilter>());
                        }
                    }
                }
            }
            //foreach (GameObject block in map)
            //{
            //    Debug.Log(block.layer);
            //    if (block.layer == walkableLayerInt)
            //    {
            //        pathMeshFilters.Add(block.GetComponent<MeshFilter>());
            //    }
            //    else
            //    {
            //        levelMeshFilters.Add(block.GetComponent<MeshFilter>());
            //    }
            //}

            CombineInstance[] combinedPathMeshes = new CombineInstance[pathMeshFilters.Count];
            for (int i = 0; i < pathMeshFilters.Count; i++)
            {
                if (pathMeshFilters[i] != null)
                {
                    combinedPathMeshes[i].mesh = pathMeshFilters[i].mesh;
                    combinedPathMeshes[i].transform = pathMeshFilters[i].transform.localToWorldMatrix;
                    Destroy(pathMeshFilters[i].gameObject);
                }
            }

            pathMeshFilter.mesh = new Mesh();
            pathMeshFilter.mesh.CombineMeshes(combinedPathMeshes);
            pathMeshRenderer.material = pathMaterial;

            CombineInstance[] combinedLevelMeshes = new CombineInstance[levelMeshFilters.Count];
            for (int i = 0; i < levelMeshFilters.Count; i++)
            {
                if (levelMeshFilters[i] != null)
                {
                    combinedLevelMeshes[i].mesh = levelMeshFilters[i].mesh;
                    combinedLevelMeshes[i].transform = levelMeshFilters[i].transform.localToWorldMatrix;
                    Destroy(levelMeshFilters[i].gameObject);
                }
            }

            levelMeshFilter.mesh = new Mesh();
            levelMeshFilter.mesh.CombineMeshes(combinedLevelMeshes);
            levelMeshRenderer.material = levelMaterial;
        }
=======
>>>>>>> bugfixes:Assets/Games/AOTVBRScripts/LevelGenerator.cs
        #endregion
    }
}