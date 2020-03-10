using System;
using UnityEngine;
using UnityEngine.AI;

namespace AOTVBR
{
    #pragma warning disable // No need to override other comparison operators
    public abstract class EnemyBase : MonoBehaviour, IComparable<EnemyBase>
    {
        public NavMeshAgent NavMeshAgent { get; private set; }
        [SerializeField]
        private new string name = "Enemy";
        public string Name { get => name; }
        [SerializeField]
        private float hitpoints = 50;
        public float Hitpoints { get => hitpoints; }
        private float originalHitpoints;
        [SerializeField]
        private int damage = 5;
        public int Damage { get => damage; }
        [SerializeField]
        private float fundAmount = 1;
        public float FundAmount { get => fundAmount; }

        private bool isDead;

        private void Awake()
        {
            originalHitpoints = hitpoints;
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            hitpoints = originalHitpoints;
            isDead = false;
        }

        public virtual void TakeDamage(float damage)
        {
            if (isDead) return; // Prevent spamming death events

            hitpoints -= damage;
            if (hitpoints <= 0)
            {
                isDead = true;
                Explosion();
                GiveFundsToPlayer();
                DeathEvent();
            }
        }

        protected virtual void Explosion()
        {
            Explosion explosion = ExplosionPool.Instance.Get();
            explosion.transform.position = transform.position;
        }

        protected virtual void GiveFundsToPlayer()
        {
            if (!PlayerData.Instance.HasMaxFunds())
            {
                PlayerData.Instance.Funds += fundAmount;
            }
        }

        public abstract void DeathEvent();

        /// <summary>
        /// Compare hitpoints equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(EnemyBase other)
        {
            if (other == null) return 1;

            if (Hitpoints > other.Hitpoints) return 1;
            else if (Hitpoints < other.Hitpoints) return -1;
            else return 0;
        }
    }
}