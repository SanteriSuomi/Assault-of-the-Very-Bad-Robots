using UnityEngine;

public class RandomLevelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject normalBlock = default;

    [SerializeField]
    private int xLength = 20;
    [SerializeField]
    private int zLength = 20;
    [Tooltip("One Y row is every 0.5.")]
    [SerializeField]
    private float yLength = 0.5f;

    private GameObject[,,] map;

    private void Awake()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        map = new GameObject[xLength, Mathf.RoundToInt(yLength + yLength), zLength];
    }

    private void Start()
    {
        InitializeMapBlocks();
        InitializePath();
    }

    private void InitializeMapBlocks()
    {
        // Instantiate the every X row.
        for (int xRow = 0; xRow < xLength; xRow++)
        {
            GameObject xObject = Instantiate(normalBlock);
            xObject.transform.position = new Vector3(xRow, 0, 0);
            // Add map array for further processing.
            map[xRow, 0, 0] = xObject;
            // Instantiate every zRow.
            for (int zRow = 0; zRow < zLength; zRow++)
            {
                GameObject zObject = Instantiate(normalBlock);
                zObject.transform.position = new Vector3(xRow, 0, zRow);
                // Add map array for further processing.
                map[xRow, 0, zRow] = zObject;
                // Instantiate every yRow.
                for (float yRow = 0; yRow < yLength; yRow += 0.5f)
                {
                    GameObject yObject = Instantiate(normalBlock);
                    yObject.transform.position = new Vector3(xRow, yRow + 0.5f, zRow);
                    // Add map array for further processing.
                    map[xRow, Mathf.RoundToInt(yRow + yRow), zRow] = yObject;
                }
            }
        }
    }

    private void InitializePath()
    {
        int randomStartPassX = Random.Range(5, 14);
        int randomStartPassAmountZ = Random.Range(4, 8);
        Debug.Log(randomStartPassX);
        Debug.Log(randomStartPassAmountZ);

        for (int i = 0; i < randomStartPassAmountZ; i++)
        {
            Destroy(map[randomStartPassX, 0, i]);
        }

        int randomSecondPassAmountX = Random.Range(3, 6);
        int randomSecondPassDirection = Random.Range(0, 2);
        Debug.Log(randomSecondPassAmountX);
        Debug.Log(randomSecondPassDirection);

        for (int i = 0; i < randomSecondPassAmountX; i++)
        {
            if (randomSecondPassDirection == 0)
            {
                print("+");
                Destroy(map[randomStartPassX + i, 0, randomStartPassAmountZ]);
            }
            else
            {
                print("-");
                Destroy(map[randomStartPassX - i, 0, randomStartPassAmountZ]);
            }
        }
        
        int randomThirdPassAmountZ = Random.Range(4, 8);
        for (int i = 0; i < randomThirdPassAmountZ; i++)
        {
            Destroy(map[randomStartPassX, 0, i]);
        }
    }
}