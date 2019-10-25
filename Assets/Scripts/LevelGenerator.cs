using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance { get; set; }

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

    private GameObject enemyBase;
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
        // Initialize the camera pivot to the correct position in the middle of the map.
        Instantiate(cameraPivot).transform.position = new Vector3(xLength / 2, Mathf.RoundToInt(yLength + yLength), zLength / 2);
    }

    private void Start()
    {
        // Start the game with a generated map.
        GenerateMap();
    }

    public void GenerateMap()
    {
        // When starting to generate a new map, introduce a small delay.
        StartCoroutine(GenerateMapTimer());
    }

    private IEnumerator GenerateMapTimer()
    {
        yield return null;
        // Clear the current map.
        ClearMap();
        // Initialize a new random seed.
        InitializeSeed();
        // Generate the necessary level blocks.
        GenerateMapBlocks();
        // Generate the enemy agent path on the level blocks.
        GenerateMapPath();
        // Generate a navmesh for the enemy agents.
        GenerateNavMesh();
        // Force a garbage collection for smoother gameplay.
        ForceCleanUp();
        // Event for hiding the generating map text.
        GeneratingTextHideEvent.Invoke();
    }

    #region Generate Map
    private void GenerateMapBlocks()
    {
        try
        {
            // Instantiate the X row of level blocks.
            for (int xRow = 0; xRow < xLength; xRow++)
            {
                InitializeXRow(xRow);
                // For every X row, instantiate a Z row.
                for (int zRow = 0; zRow < zLength; zRow++)
                {
                    InitializeZRow(xRow, zRow);
                    // For every X and Z row, instantiate Y row.
                    for (float yRow = 0; yRow < yLength; yRow += 0.5f)
                    {
                        InitializeYRow(xRow, zRow, yRow);
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

    private void InitializeXRow(int xRow)
    {
        GameObject xObject = Instantiate(levelPathPrefab);
        xObject.transform.parent = levelPrefabParent;
        xObject.layer = 9;
        xObject.transform.position = new Vector3(xRow, 0, 0);
        // Add objects to the multi-dimensional array of level blocks.
        map[xRow, 0, 0] = xObject;
    }

    private void InitializeZRow(int xRow, int zRow)
    {
        GameObject zObject = Instantiate(levelPathPrefab);
        zObject.transform.parent = levelPrefabParent;
        zObject.layer = 9;
        zObject.transform.position = new Vector3(xRow, 0, zRow);
        map[xRow, 0, zRow] = zObject;
    }

    private void InitializeYRow(int xRow, int zRow, float yRow)
    {
        GameObject yObject = Instantiate(levelPrefab);
        yObject.transform.parent = levelPrefabParent;
        yObject.layer = 11;
        // Make sure the objects above the level ground are carving for the navmesh.
        yObject.AddComponent<NavMeshObstacle>().carving = true;
        yObject.transform.position = new Vector3(xRow, yRow + 0.5f, zRow);
        map[xRow, Mathf.RoundToInt(yRow + yRow), zRow] = yObject;
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
            GenerateFirstPass(out int firstPassX, out int firstPassAmountZ);
            // Spawn the enemy ship.
            SpawnShip();
            // Indicate that agentStartPoint can be set again.
            setAgentStartPoint = false;
            GenerateSecondPass(firstPassX, firstPassAmountZ);
            GenerateThirdPass(firstPassAmountZ);
            GenerateFourthPass();
            Vector3 agentEndPoint = GenerateFifthPassAndGetAgentEndPoint();
            // Spawn the player base at the agent end point (last point on the path).
            SpawnBase(agentEndPoint);
            // Set the enemy agent end point.
            LevelData.Instance.AgentEndPoint = agentEndPoint;
        }
        catch (System.Exception e)
        {
            #if UNITY_EDITOR
            Debug.Log(e);
            #endif
            // If the generation algorithm goes off the array, generate the map again.
            GenerateMap();
        }
    }

    private void GenerateFirstPass(out int firstPassX, out int firstPassAmountZ)
    {
        // Generate the first pass of the path.
        // Starting from X position on the array.
        firstPassX = Random.Range(2, xLength - 2);
        // Agent start point should be the first position in the path.
        SetAgentStartPoint(firstPassX);
        // How much we need to carve in Z axis.
        firstPassAmountZ = Random.Range(2, zLength / 2);
        for (int i = 0; i < firstPassAmountZ; i++)
        {
            // Carve the path in the array.
            Destroy(map[firstPassX, 0, i]);
        }
    }

    private void GenerateSecondPass(int firstPassX, int firstPassAmountZ)
    {
        // Second pass for generating the path.
        // How much should we generate.
        int secondPassAmountX = Random.Range(2, xLength - 2);
        // Direction of the carve on X axis.
        int secondPassDirection = Random.Range(0, 2);
        for (int i = 0; i < secondPassAmountX; i++)
        {
            // Determine the direction.
            if (secondPassDirection == 0)
            {
                // Start the pass at 0.
                thirdPassX = 0;
                // Carve the path starting from the firstpass ending position.
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
    }

    private void GenerateThirdPass(int firstPassAmountZ)
    {
        // Third pass amount in Z axis.
        int thirdPassAmountZ = Random.Range(2, zLength / 2);
        for (int i = 0; i < thirdPassAmountZ; i++)
        {
            fourthPassZ = 0;
            // Start the fourth pass at the Z position from first pass.
            fourthPassZ = firstPassAmountZ + i;
            Destroy(map[thirdPassX, 0, firstPassAmountZ + i]);
        }
    }

    private void GenerateFourthPass()
    {
        // Fourth pass, again in X axis on different direction.
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
    }

    private Vector3 GenerateFifthPassAndGetAgentEndPoint()
    {
        // Last generation pass, going to the end of the map in Z axis.
        // Make sure to generate the path to the end of the map.
        int fifthPassAmountZ = zLength - fourthPassZ;
        // Initialize the agent end point with 0.
        Vector3 agentEndPoint = Vector3.zero;
        for (int i = 0; i < fifthPassAmountZ; i++)
        {
            Destroy(map[fifthPassX, 0, fourthPassZ + i]);
            // Make sure the agent end point is the last path block to be generated.
            agentEndPoint = SetCalculateAgentEndPoint(agentEndPoint, i);
        }

        return agentEndPoint;
    }

    private void SpawnShip()
    {
        // If the ship exists currently, destroy it.
        if (shipPrefab != null)
        {
            Destroy(enemyBase);
        }

        enemyBase = Instantiate(shipPrefab);
        // Correctly rotate the ship prefab.
        enemyBase.transform.rotation = Quaternion.Euler(0, 270, 0);
        enemyBase.transform.position = LevelData.Instance.AgentStartPoint + new Vector3(0.08f, 0, -2.15f);
    }

    private void SpawnBase(Vector3 agentEndPoint)
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
    private void ClearMap()
    {
        // When generating a new map, destroy all the blocks currently existing.
        foreach (GameObject block in map)
        {
            Destroy(block);
        }
    }

    private void InitializeSeed()
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

    private void SetAgentStartPoint(int firstPassX)
    {
        // Set the start point for the enemy agents.
        if (!setAgentStartPoint)
        {
            setAgentStartPoint = true;
            LevelData.Instance.AgentStartPoint = map[firstPassX, 0, 0].transform.position;
        }
    }

    private Vector3 SetCalculateAgentEndPoint(Vector3 agentEndPoint, int i)
    {
        // Calculate the agent endpoint from the fifth pass in the path generation.
        if (map[fifthPassX, 0, fourthPassZ + i].transform.position.z > agentEndPoint.z)
        {
            agentEndPoint = map[fifthPassX, 0, fourthPassZ + i].transform.position;
        }

        return agentEndPoint;
    }
    #endregion
}