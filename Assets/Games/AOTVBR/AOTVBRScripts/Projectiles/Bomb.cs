using UnityEngine;

namespace AOTVBR
{
    public class Bomb : ProjectileBase
    {
        private Collider[] hitEnemies;
        [SerializeField]
        private int hitEnemiesArrayLength = 10;
        [SerializeField]
        private LayerMask bombDamageMask = default;

        [SerializeField]
        private float damageRadius = 2;
        [SerializeField]
        private float bombDamage = 5;

        protected override void Awake()
        {
            base.Awake();
            hitEnemies = new Collider[hitEnemiesArrayLength];
        }

        protected override void DeactivateDelay()
            => TowerBombBombPool.Instance.Return(this);

        protected override void OnCollisionEnter(Collision collision)
        {
            Explosion explosion = ExplosionPool.Instance.Get();
            explosion.transform.position = transform.position;
            DealDamageInRadius();
            TowerBombBombPool.Instance.Return(this);
        }

        private void DealDamageInRadius()
        {
            int hitEnemiesAmount = Physics.OverlapSphereNonAlloc(transform.position, damageRadius,
                hitEnemies, bombDamageMask);
            if (hitEnemiesAmount > 0)
            {
                for (int i = 0; i < hitEnemies.Length; i++)
                {
                    if (hitEnemies[i] != null
                        && hitEnemies[i].TryGetComponent(out EnemyBase enemy))
                    {
                        enemy.TakeDamage(bombDamage);
                    }
                }
            }
        }
    }
}