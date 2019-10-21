using UnityEngine;

public class LevelData : MonoBehaviour
{
    public static LevelData Instance { get; set; }

    private Vector3 agentStartPoint;
    public Vector3 AgentStartPoint
    {
        get { return agentStartPoint; }
        set
        {
            agentStartPoint = value;

            #if UNITY_EDITOR
            Debug.Log(agentStartPoint);
            #endif
        }
    }

    private Vector3 agentEndPoint;
    public Vector3 AgentEndPoint
    {
        get { return agentEndPoint; }
        set
        {
            agentEndPoint = value;

            #if UNITY_EDITOR
            Debug.Log(agentEndPoint);
            #endif
        }
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
