using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AOTVBR
{
    public class TowerBomb : TowerBase
    {
        [SerializeField]
        private float bombAttackInterval = 2;
        private float bombAttackTimer;
        
        protected override void EnemyDetectedEvent(Vector3 enemyPosition)
        {
            bombAttackTimer += Time.deltaTime;
            if (bombAttackTimer >= bombAttackInterval)
            {
                bombAttackTimer = 0;
            }
        }

        protected override void DamageEnemyOnDetection(EnemyBase enemy)
        {
            // Disable regular attacking on Bomb Tower (bombs deal damage).
        }

        protected override void PlayEnemyDetectedAttackAudio()
        {
            throw new System.NotImplementedException();
        }

        protected override void ResetTower()
        {
            base.ResetTower();
            bombAttackTimer = 0;
        }
    }
}