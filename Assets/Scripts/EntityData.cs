using System.Collections.Generic;
using UnityEngine;

public class EntityData : MonoBehaviour
{
    public static EntityData Instance { get; set; }

    public List<GameObject> ActiveMapEntityList { get; set; } = new List<GameObject>();

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
