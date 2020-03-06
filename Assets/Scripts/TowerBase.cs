using UnityEngine;

namespace AOTVBR
{
    public abstract class TowerBase : MonoBehaviour
    {
        protected enum EnemyType
        {
            TowerBeam,
            TowerGun
        }
        [SerializeField]
        protected EnemyType enemyType = default;
        [SerializeField]
        protected LayerMask attackableEnemiesLayers = default;

        [SerializeField]
        private new string name = "Tower";
        public string Name { get; set; }
        [SerializeField]
        private int cost = 10;
        public int Cost { get; set; }
        [SerializeField]
        private float damage = 10;
        public float Damage { get; set; }

        protected bool isPlacing;

        public void IsPlacing(bool enable) 
            => isPlacing = enable;

        private void Awake()
        {
            Name = name;
            Cost = cost;
            Damage = damage;
        }
    }
}