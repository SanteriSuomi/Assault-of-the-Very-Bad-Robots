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
        [SerializeField]
        private float startingFundsMultiplier = 1;

        public int Health { get; set; }
        public int StartingHealth { get; private set; }
        private float funds;
        public float Funds
        {
            get => funds;
            set => funds = value * FundsMultiplier;
        }
        public float FundsMultiplier { get; set; }
        public float StartingFundsMultiplier { get; private set; }
        public float StartingFunds { get; private set; }
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
            StartingHealth = startingHealth;
            MaxFunds = maxFunds;
            FundsMultiplier = startingFundsMultiplier;
            StartingFundsMultiplier = startingFundsMultiplier;
            Funds = startingFunds;
            StartingFunds = startingFunds;
        }
    }
}