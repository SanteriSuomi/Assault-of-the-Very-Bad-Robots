using UnityEngine;

public class RandomLevelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject normalBlock = default;
    private GameObject[,,] map;

    [SerializeField]
    private int xLength = 20;
    [SerializeField]
    private int zLength = 20;
    [Tooltip("One Y row is every 0.5.")]
    [SerializeField]
    private float yLength = 0.5f;

    private void Awake()
    {
        // Initialize the map matrix.
        map = new GameObject[xLength, Mathf.RoundToInt(yLength + yLength), zLength];
    }

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        ClearMap();

        InitializeSeed();
        GenerateMapBlocks();
        GenerateMapPath();
    }

    private void GenerateMapBlocks()
    {
        // Instantiate the every X row.
        for (int xRow = 0; xRow < xLength; xRow++)
        {
            GameObject xObject = Instantiate(normalBlock);
            xObject.transform.position = new Vector3(xRow, 0, 0);
            // Add to map array for further processing.
            map[xRow, 0, 0] = xObject;
            // Instantiate every Z row.
            for (int zRow = 0; zRow < zLength; zRow++)
            {
                GameObject zObject = Instantiate(normalBlock);
                zObject.transform.position = new Vector3(xRow, 0, zRow);
                // Add to map array for further processing.
                map[xRow, 0, zRow] = zObject;
                // Instantiate every Y row.
                for (float yRow = 0; yRow < yLength; yRow += 0.5f)
                {
                    GameObject yObject = Instantiate(normalBlock);
                    yObject.transform.position = new Vector3(xRow, yRow + 0.5f, zRow);
                    // Add to map array for further processing.
                    map[xRow, Mathf.RoundToInt(yRow + yRow), zRow] = yObject;
                }
            }
        }
    }

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
                Destroy(map[firstPassX, 0, i]);
            }

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
            for (int i = 0; i < fifthPassAmountZ; i++)
            {
                Destroy(map[fifthPassX, 0, fourthPassZ + i]);
            }
        }
        catch (System.Exception e)
        {
            #if UNITY_EDITOR
            Debug.Log(e);
            #endif

            ClearMap();
            GenerateMap();
        }
    }

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
}