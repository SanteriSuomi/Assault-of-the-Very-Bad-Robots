using System.Collections.Generic;
using UnityEngine;

public class EntityData : MonoBehaviour
{
    public static EntityData Instance { get; set; }
    // Stores data for currently active entities on the map (towers, enemies etc).
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
