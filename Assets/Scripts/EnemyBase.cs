using System;
using UnityEngine;

namespace AOTVBR
{
    public abstract class EnemyBase : MonoBehaviour, IComparable<EnemyBase>
    {
        [SerializeField]
        private new string name = "Enemy";
        public string Name { get => name; }

        [SerializeField]
        private float hitpoints = 50;
        public float Hitpoints { get => hitpoints; }

        [SerializeField]
        private int damage = 5;
        public int Damage { get => damage; }

        [SerializeField]
        private float fundAmount = 1;
        public float FundAmount { get => fundAmount; }

        private const float MaxFunds = 30;

        [SerializeField]
        protected GameObject explosionPrefab = default;

        public virtual void TakeDamage(float damage)
        {
            hitpoints -= damage;
            if (hitpoints <= 0)
            {
                Die();
                FundPlayer();
            }
        }

        public virtual void Die()
        {
            Explosion();
            Destroy(gameObject);
        }

        protected virtual void Explosion()
            => Instantiate(explosionPrefab).transform.position = transform.position;

        protected virtual void FundPlayer()
        {
            if (PlayManager.Instance.Funds < MaxFunds)
            {
                PlayManager.Instance.Funds += fundAmount;
            }
        }

        /// <summary>
        /// Compare hitpoints equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(EnemyBase other)
        {
            if (Mathf.Approximately(Hitpoints, other.Hitpoints)) return 0;
            else if (Hitpoints > other.Hitpoints) return 1;
            else return -1;
        }
    }
}