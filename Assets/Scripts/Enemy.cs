using UnityEngine;

namespace AOTVBR
{
    public class Enemy : MonoBehaviour, IDamageable, IDoDamage, ICanGiveFunds, IHasName
    {
        [SerializeField]
        private new string name = "Enemy";
        public string Name { get; set; }

        public IHasName GetIHasName() => this;

        [SerializeField]
        private float hitpoints = 50;
        public float Hitpoints { get; set; }

        [SerializeField]
        private int damage = 5;
        public int Damage { get; set; }

        [SerializeField]
        private float fundAmount = 1;
        public float FundAmount { get; set; }

        private static float maxFunds = 30;

        [SerializeField]
        private GameObject explosionPrefab = default;

        private void Awake()
        {
            // Apply the serialized values for this enemy instance.
            Name = name;
            Hitpoints = hitpoints;
            Damage = damage;
            FundAmount = fundAmount;
        }

        public void TakeDamage(float damage)
        {
            Hitpoints -= damage;
            if (Utility.FloatEqual(Hitpoints, 0))
            {
                Die();
                GiveFunds();
            }
        }

        public void Die()
        {
            Explosion();
            Destroy(gameObject);
        }

        private void Explosion() 
            => Instantiate(explosionPrefab).transform.position = transform.position;

        public void GiveFunds()
        {
            if (PlayManager.Instance.Funds < maxFunds)
            {
                PlayManager.Instance.Funds += fundAmount;
            }
        }
    } 
}