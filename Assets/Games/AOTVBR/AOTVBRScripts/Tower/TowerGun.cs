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
        [Range(0, 1)]
        private float rotationSpeed = 0.75f;
        [SerializeField]
        private float bulletSpeed = 10;
        [SerializeField]
        private float bulletShootInterval = 1;
        private float bulletShootTimer;
        [SerializeField]
        private float minimumDotProductToFire = -0.95f;

        private bool playGunShot;

        private void Awake()
            => SetInitialRotation();

        private void SetInitialRotation()
            => turret.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        protected override void EnemyDetectedEvent(Vector3 enemyPosition)
        {
            RotateTurret(target: enemyPosition);
            ShootBullet(target: enemyPosition);
        }

        private void RotateTurret(Vector3 target)
        {
            Vector3 direction = target - turret.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            turret.rotation = Quaternion.Slerp(turret.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        private void ShootBullet(Vector3 target)
        {
            bulletShootTimer += Time.deltaTime;
            if (bulletShootTimer >= bulletShootInterval)
            {
                Vector3 directionToTarget = (turret.position - target).normalized;
                if (Vector3.Dot(turret.forward, directionToTarget) <= minimumDotProductToFire)
                {
                    playGunShot = true;
                    bulletShootTimer = 0;
                    SpawnBulletAt(bulletHoleLeft);
                    SpawnBulletAt(bulletHoleRight);
                }
            }
        }

        private void SpawnBulletAt(Transform transform)
        {
            Bullet bulletObj = TowerGunBulletPool.Instance.Get();
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