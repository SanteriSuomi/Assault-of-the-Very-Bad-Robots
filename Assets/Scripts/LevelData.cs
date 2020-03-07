using UnityEngine;

namespace AOTVBR
{
    public class LevelData : Singleton<LevelData>
    {
        public Vector3 AgentStartPoint { get; set; }
        public Vector3 AgentEndPoint { get; set; }
    } 
}