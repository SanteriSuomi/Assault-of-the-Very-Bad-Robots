using UnityEngine;

namespace AOTVBR
{
    public class TowerBomb : TowerBase
    {
        [SerializeField]
        private Transform bombHole = default;

        [SerializeField]
        private float bombAttackInterval = 2;
        private float bombAttackTimer;
        [SerializeField]
        private float rotationSpeed = 1;
        [SerializeField]
        private float bombShootSpeedMultiplier = 2;
        [SerializeField]
        private float maxDistanceForVelocityHalve = 2;

        private bool playBombShoot;
        
        private void Awake()
            => SetInitialRotation();

        protected override void EnemyDetectedEvent(Vector3 enemyPosition)
        {
            RotateTurret(enemyPosition, rotationSpeed, true);
            bombAttackTimer += Time.deltaTime;
            if (bombAttackTimer >= bombAttackInterval 
                && IsFacingTarget(turret.forward, enemyPosition))
            {
                playBombShoot = true;
                bombAttackTimer = 0;
                ShootBomb(enemyPosition);
            }
        }

        private void ShootBomb(Vector3 enemyPosition)
        {
            Bomb bomb = BombPool.Instance.Get();
            bomb.transform.position = bombHole.transform.position;
            float distanceToEnemy = (enemyPosition - transform.position).magnitude;
            if (distanceToEnemy <= maxDistanceForVelocityHalve) // Have speed for lower distance so they don't go too far.
            {
                ApplyVelocity(bomb, distanceToEnemy, bombShootSpeedMultiplier / 2);
            }
            else
            {
                ApplyVelocity(bomb, distanceToEnemy, bombShootSpeedMultiplier);
            }
        }

        private void ApplyVelocity(Bomb bomb, float distanceToEnemy, float multiplier)
        {
            bomb.Rigidbody.velocity = bombHole.transform.forward
                * distanceToEnemy
                * multiplier;
        }

        protected override void DamageEnemyOnDetection(EnemyBase enemy)
        {
            // Disable base damage.
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