using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelPool : MonoBehaviour
{
    public static LevelPool Instance { get; set; }

    [SerializeField]
    private GameObject levelPrefab = default;
    [SerializeField]
    private Transform levelPrefabParent = default;
    private List<GameObject> levelPool = new List<GameObject>();

    private int poolAmount = 400;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            GameObject levelObject = Instantiate(levelPrefab);
            levelObject.transform.parent = levelPrefabParent;
            levelObject.SetActive(false);
            levelPool.Add(levelObject);
            print(levelObject);
        }
    }

    public void DeactivateObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public GameObject GetObject() => levelPool.Where(g => !g.activeSelf).FirstOrDefault();
}
