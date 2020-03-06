using UnityEngine;

namespace AOTVBR
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [SerializeField]
        protected new string name = "Enemy";
        public string Name { get => name; }

        [SerializeField]
        protected float hitpoints = 50;
        public float Hitpoints { get => hitpoints; }

        [SerializeField]
        protected int damage = 5;
        public int Damage { get => damage; }

        [SerializeField]
        protected float fundAmount = 1;
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
    } 
}