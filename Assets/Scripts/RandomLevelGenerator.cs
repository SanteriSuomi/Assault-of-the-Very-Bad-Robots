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
        map = new GameObject[xLength, Mathf.RoundToInt(yLength + yLength), zLength];
    }

    private void Start()
    {
        // Instantie the every X row.
        for (int xRow = 0; xRow < xLength; xRow++)
        {
            GameObject xObject = Instantiate(normalBlock);
            xObject.transform.position = new Vector3(xRow, 0, 0);

            map[xRow, 0, 0] = xObject;

            // Instantiate every zRow.
            for (int zRow = 0; zRow < zLength; zRow++)
            {
                GameObject zObject = Instantiate(normalBlock);
                zObject.transform.position = new Vector3(xRow, 0, zRow);

                map[xRow, 0, zRow] = zObject;

                // Instantiate every yRow.
                for (float yRow = 0; yRow < yLength; yRow += 0.5f)
                {
                    GameObject yObject = Instantiate(normalBlock);
                    zObject.transform.position = new Vector3(xRow, yRow + 0.5f, zRow);

                    map[xRow, Mathf.RoundToInt(yRow + yRow), zRow] = yObject;
                }
            }
        }

        foreach (var item in map)
        {
            print(item.name.ToString());
        }
    }

    private void Update()
    {

    }
}
