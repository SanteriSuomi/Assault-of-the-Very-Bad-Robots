using UnityEngine;

namespace AOTVBR
{
    public class PlayerData : Singleton<PlayerData>
    {
        [SerializeField]
        private int startingHealth = 100;
        [SerializeField]
        private int startingFunds = 10;
        [SerializeField]
        private int maxFunds = 30;

        public int Health { get; set; }
        public float Funds { get; set; }
        public int MaxFunds { get; private set; }

        public bool HasMaxFunds()
        {
            if (Funds >= MaxFunds)
            {
                return true;
            }

            return false;
        }

        protected override void Awake()
        {
            Health = startingHealth;
            Funds = startingFunds;
            MaxFunds = maxFunds;
        }
    }
}