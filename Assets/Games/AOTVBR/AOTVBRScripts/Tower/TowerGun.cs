using UnityEngine;

namespace AOTVBR
{
    public class TowerGun : TowerBase
    {
        [SerializeField]
        private Transform bulletHoleLeft = default;
        [SerializeField]
        private Transform bulletHoleRight = default;

        [SerializeField]
        private float rotationSpeed = 1;
        [SerializeField]
        private float bulletSpeed = 10;
        [SerializeField]
        private float bulletShootInterval = 1;
        private float bulletShootTimer;

        private bool playGunShot;

        private void Awake()
            => SetInitialRotation();

        protected override void EnemyDetectedEvent(Vector3 enemyPosition)
        {
            RotateTurret(target: enemyPosition, rotationSpeed, false);
            ShootBullet(target: enemyPosition);
        }

        private void ShootBullet(Vector3 target)
        {
            bulletShootTimer += Time.deltaTime;
            if (bulletShootTimer >= bulletShootInterval
                && IsFacingTarget(turret.forward, target))
            {
                playGunShot = true;
                bulletShootTimer = 0;
                SpawnBulletAt(bulletHoleLeft);
                SpawnBulletAt(bulletHoleRight);
            }
        }

        private void SpawnBulletAt(Transform transform)
        {
            Bullet bulletObj = BulletPool.Instance.Get();
            bulletObj.transform.position = transform.position;
            bulletObj.Rigidbody.velocity = transform.forward * bulletSpeed;
        }

        protected override void PlayAttackAudio()
        {
            if (playGunShot)
            {
                playGunShot = false;
                audioSource.Play();
            }
        }
    }
}