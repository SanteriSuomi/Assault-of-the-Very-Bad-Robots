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

        private bool playBombShoot;
        
        protected override void EnemyDetectedEvent(Vector3 enemyPosition)
        {

        }

        protected override void DamageEnemyOnDetection(EnemyBase enemy)
        {
            bombAttackTimer += Time.deltaTime;
            if (bombAttackTimer >= bombAttackInterval)
            {
                playBombShoot = true;
                bombAttackTimer = 0;
            }
        }

        protected override void PlayAttackAudio()
        {
            if (playBombShoot)
            {
                playBombShoot = false;
                audioSource.Play();
            }
        }

        protected override void ResetTower()
        {
            base.ResetTower();
            bombAttackTimer = 0;
        }
    }
}