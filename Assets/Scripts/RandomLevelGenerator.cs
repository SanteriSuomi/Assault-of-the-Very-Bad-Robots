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

    private int startPassArrayIntX;
    private int startPassArrayIntZ;

    private void Awake()
    {
        map = new GameObject[xLength, Mathf.RoundToInt(yLength + yLength), zLength];
    }

    private void Start()
    {
        InitializeMapBlocks();
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

        InitializePath();
    }

    private void InitializePath()
    {
        int randomStartPass = Random.Range(1, 19);
        int randomStartPassAmount = Random.Range(4, 8);

        for (int i = 0; i < randomStartPassAmount; i++)
        {
            startPassArrayIntX = i;
            Destroy(map[randomStartPass, 0, i]);
        }

        print(startPassArrayIntX);
        
        int randomSecondPassAmount = Random.Range(4, 8);
        for (int i = 0; i < randomSecondPassAmount; i++)
        {
            Destroy(map[startPassArrayIntX + i, 0, startPassArrayIntZ]);
        }
        
        /*
        Vector3 startPos = map[Random.Range(0, xLength), (int)yLength, 0].transform.position;
        print(startPos);
        Vector3 endPos = map[Random.Range(0, xLength), (int)yLength, zLength - 1].transform.position;
        print(endPos);
        float distance = Vector3.Distance(startPos, endPos);
        print(distance);

        RaycastHit[] raycastHits = Physics.RaycastAll(startPos, endPos, distance);
        foreach (var hit in raycastHits)
        {
            Destroy(hit.collider.gameObject);
        }
        */
    }
}