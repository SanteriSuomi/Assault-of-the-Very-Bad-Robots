using System.Collections.Generic;
using UnityEngine;

namespace AOTVBR
{
    public class EntityData : Singleton<EntityData>
    {
        // Stores data for currently active entities on the map (towers, enemies etc).
        public List<GameObject> ActiveMapEntityList { get; } = new List<GameObject>();
    } 
}
