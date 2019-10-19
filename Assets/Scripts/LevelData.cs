using UnityEngine;

public class LevelData : MonoBehaviour
{
    public static LevelData Instance { get; set; }
    public Vector3 AgentStartPoint { get; set; }
    public Vector3 AgentEndPoint { get; set; }

    private void Update()
    {
        print(AgentStartPoint);
    }

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
    }
}
