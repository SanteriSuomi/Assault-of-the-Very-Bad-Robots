﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

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

        [SerializeField]
        private Quaternion playerBaseStartRotation = Quaternion.Euler(0, 180, 0);
        [SerializeField]
        private Vector3 playerBaseStartPositionOffset = new Vector3(0, 0, 3.695f);

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

        protected override void Awake()
            => map = new GameObject[xLength, Mathf.RoundToInt(yLength + yLength), zLength];

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
            List<MeshFilter> blockMeshes = GenerateMapBlocks();
            GenerateMapPath();
            GenerateNavMesh();
            ForceCleanUp();
            CombineMap();
            GeneratingTextHideEvent.Invoke();
        }

        #region Generate Map
        private List<MeshFilter> GenerateMapBlocks()
        {
            List<MeshFilter> blockMeshes = new List<MeshFilter>();
            for (int x = 0; x < xLength; x++)
            {
                InitializeX(blockMeshes, x);
                for (int z = 0; z < zLength; z++)
                {
                    InitializeZ(blockMeshes, x, z);
                    for (float y = 0; y < yLength; y += yInitializationOffset)
                    {
                        InitializeY(blockMeshes, x, z, y);
                    }
                }
            }

            return blockMeshes;
        }

        private void InitializeX(List<MeshFilter> blockMeshes, int x)
        {
            GameObject xObj = Instantiate(levelPathPrefab);
            xObj.transform.SetParent(levelPrefabParent);
            xObj.layer = walkableLayerInt;
            xObj.transform.position = new Vector3(x, 0, 0);
            blockMeshes.Add(xObj.GetComponent<MeshFilter>());
            map[x, 0, 0] = xObj;
        }

        private void InitializeZ(List<MeshFilter> blockMeshes, int x, int z)
        {
            GameObject zObj = Instantiate(levelPathPrefab);
            zObj.transform.SetParent(levelPrefabParent);
            zObj.layer = walkableLayerInt;
            zObj.transform.position = new Vector3(x, 0, z);
            blockMeshes.Add(zObj.GetComponent<MeshFilter>());
            map[x, 0, z] = zObj;
        }

        private void InitializeY(List<MeshFilter> blockMeshes, int x, int z, float y)
        {
            GameObject yObj = Instantiate(levelPrefab);
            yObj.transform.SetParent(levelPrefabParent);
            yObj.layer = nonWalkableLevelLayerInt;
            yObj.AddComponent<NavMeshObstacle>().carving = true; // Level blocks that are not the path should not have navmesh.
            yObj.transform.position = new Vector3(x, y + yInitializationOffset, z);
            blockMeshes.Add(yObj.GetComponent<MeshFilter>());
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

        private int GenerateFifthPath() // Last path
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
                enemyShipBase.transform.rotation = enemyShipBaseStartRotation;
                enemyShipBase.transform.position =
                    LevelData.Instance.AgentStartPoint + enemyShipBaseStartPositionOffset;
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
                playerBase.transform.rotation = playerBaseStartRotation;
                playerBase.transform.position = 
                    LevelData.Instance.AgentEndPoint + playerBaseStartPositionOffset;
            }
        }
        #endregion

        #region Other Map Methods
        private void ClearCurrentMap()
        {
            foreach (GameObject mapBlock in map)
            {
                if (mapBlock != null)
                {
                    Destroy(mapBlock);
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

        private void CombineMap(List<MeshFilter> blockMeshes)
        {

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
    }
}