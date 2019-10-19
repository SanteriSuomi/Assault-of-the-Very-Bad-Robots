using System.Collections.Generic;
using UnityEngine;

public class LevelPool : MonoBehaviour
{
    public static LevelPool Instance { get; set; }

    [SerializeField]
    private GameObject levelPrefab = default;
    private Queue<GameObject> levelPool;

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

        levelPool = new Queue<GameObject>();
        int poolAmount = RandomLevelGenerator.Instance.XLength * RandomLevelGenerator.Instance.ZLength;
        for (int i = 0; i < poolAmount; i++)
        {
            GameObject levelObject = Instantiate(levelPrefab);
            levelPool.Enqueue(levelObject);
        }
    }

    public void Enqueue(GameObject gameObject)
    {
        levelPool.Enqueue(gameObject);
    }

    public GameObject Dequeue()
    {
        return levelPool.Dequeue();
    }
}
