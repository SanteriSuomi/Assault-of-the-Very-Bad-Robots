using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RandomLevelGenerator : MonoBehaviour
{
    public static RandomLevelGenerator Instance { get; set; }

    public delegate void GeneratingTextHide();
    public event GeneratingTextHide GeneratingTextHideEvent;

    [SerializeField]
    private GameObject levelPrefab = default;
    [SerializeField]
    private Transform levelPrefabParent = default;
    [SerializeField]
    private GameObject cameraPivot = default;
    [SerializeField]
    private NavMeshSurface navMesh = default;
    [SerializeField]
    private GameObject basePrefab = default;
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

    private bool setAgentStartPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        map = new GameObject[xLength, Mathf.RoundToInt(yLength + yLength), zLength];
    }

    private void OnEnable()
    {
        Instantiate(cameraPivot).transform.position = new Vector3(xLength / 2, yLength, zLength / 2);
    }

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        StartCoroutine(GenerateMapTimer());
    }

    private IEnumerator GenerateMapTimer()
    {
        yield return null;
        ClearMap();
        InitializeSeed();
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
            for (int xRow = 0; xRow < xLength; xRow++)
            {
                GameObject xObject = Instantiate(levelPrefab);
                xObject.transform.parent = levelPrefabParent;
                xObject.layer = 9;
                xObject.transform.position = new Vector3(xRow, 0, 0);
                map[xRow, 0, 0] = xObject;

                for (int zRow = 0; zRow < zLength; zRow++)
                {
                    GameObject zObject = Instantiate(levelPrefab);
                    zObject.transform.parent = levelPrefabParent;
                    zObject.layer = 9;
                    zObject.transform.position = new Vector3(xRow, 0, zRow);
                    map[xRow, 0, zRow] = zObject;

                    for (float yRow = 0; yRow < yLength; yRow += 0.5f)
                    {
                        GameObject yObject = Instantiate(levelPrefab);
                        yObject.transform.parent = levelPrefabParent;
                        yObject.layer = 11;
                        yObject.AddComponent<NavMeshObstacle>().carving = true;
                        yObject.transform.position = new Vector3(xRow, yRow + 0.5f, zRow);
                        map[xRow, Mathf.RoundToInt(yRow + yRow), zRow] = yObject;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            #if UNITY_EDITOR
            Debug.Log(e);
            #endif
        }
    }
    #endregion

    #region Generate Path
    private int thirdPassX;
    private int fourthPassZ;
    private int fifthPassX;

    private void GenerateMapPath()
    {
        try
        {
            int firstPassX = Random.Range(2, xLength - 2);
            int firstPassAmountZ = Random.Range(2, zLength / 2);
            for (int i = 0; i < firstPassAmountZ; i++)
            {
                SetAgentStartPoint(firstPassX);
                Destroy(map[firstPassX, 0, i]);
            }

            setAgentStartPoint = false;

            int secondPassAmountX = Random.Range(2, xLength - 2);
            int secondPassDirection = Random.Range(0, 2);
            for (int i = 0; i < secondPassAmountX; i++)
            {
                if (secondPassDirection == 0)
                {
                    thirdPassX = 0;
                    thirdPassX = firstPassX + i;
                    Destroy(map[firstPassX + i, 0, firstPassAmountZ]);
                }
                else
                {
                    thirdPassX = 0;
                    thirdPassX = firstPassX - i;
                    Destroy(map[firstPassX - i, 0, firstPassAmountZ]);
                }
            }

            int thirdPassAmountZ = Random.Range(2, zLength / 2);
            for (int i = 0; i < thirdPassAmountZ; i++)
            {
                fourthPassZ = 0;
                fourthPassZ = firstPassAmountZ + i;
                Destroy(map[thirdPassX, 0, firstPassAmountZ + i]);
            }

            int fourthPassAmountX = Random.Range(2, xLength - 2);
            int fourthPassDirection = Random.Range(0, 2);
            for (int i = 0; i < fourthPassAmountX; i++)
            {
                if (fourthPassDirection == 0)
                {
                    fifthPassX = 0;
                    fifthPassX = thirdPassX + i;
                    Destroy(map[thirdPassX + i, 0, fourthPassZ]);
                }
                else
                {
                    fifthPassX = thirdPassX - i;
                    Destroy(map[thirdPassX - i, 0, fourthPassZ]);
                }
            }

            int fifthPassAmountZ = zLength - fourthPassZ;
            Vector3 agentEndPoint = Vector3.zero;
            for (int i = 0; i < fifthPassAmountZ; i++)
            {
                Destroy(map[fifthPassX, 0, fourthPassZ + i]);

                agentEndPoint = SetAgentEndPoint(agentEndPoint, i);
            }
            
            if (playerBase != null)
            {
                Destroy(playerBase);
            }
            playerBase = Instantiate(basePrefab);
            playerBase.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            playerBase.transform.position = agentEndPoint + new Vector3(0, 0.175f, 3.65f);
            
            LevelData.Instance.AgentEndPoint = agentEndPoint;
        }
        catch (System.Exception e)
        {
            #if UNITY_EDITOR
            Debug.Log(e);
            #endif

            GenerateMap();
        }
    }
    #endregion

    #region Other Map Methods
    private void ClearMap()
    {
        foreach (GameObject block in map)
        {
            Destroy(block);
        }
    }

    private void InitializeSeed()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    private void GenerateNavMesh()
    {
        navMesh.BuildNavMesh();
    }

    private void ForceCleanUp()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    private void SetAgentStartPoint(int firstPassX)
    {
        if (!setAgentStartPoint)
        {
            setAgentStartPoint = true;
            LevelData.Instance.AgentStartPoint = map[firstPassX, 0, 0].transform.position;
        }
    }

    private Vector3 SetAgentEndPoint(Vector3 agentEndPoint, int i)
    {
        if (map[fifthPassX, 0, fourthPassZ + i].transform.position.z > agentEndPoint.z)
        {
            agentEndPoint = map[fifthPassX, 0, fourthPassZ + i].transform.position;
        }

        return agentEndPoint;
    }
    #endregion
}