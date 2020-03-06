using System.Linq;
using UnityEngine;

namespace AOTVBR
{
    public class Tower : TowerBase
    {
        [SerializeField]
        private Transform turret = default;
        [SerializeField]
        private GameObject bullet = default;
        [SerializeField]
        private Transform bulletHole1 = default;
        [SerializeField]
        private Transform bulletHole2 = default;
        private LineRenderer lineRenderer;
        private AudioSource audioSource;

        [SerializeField]
        private float damageTimerAmount = 1;
        [SerializeField]
        private float checkRadius = 3.25f;
        private float timer;
        [SerializeField]
        [Range(0, 1)]
        private float rotationSpeed = 0.75f;
        [SerializeField]
        private float bulletSpeed = 10;
        [SerializeField]
        private float bulletShootTime = 0.5f;
        private float bulletShootTimer;

        private bool playGunShot;

        private void Start()
        {
            InitializeTower();
        }

        private void InitializeTower()
        {
            audioSource = GetComponent<AudioSource>();
            // Initialize some stuff depending on the tower type.
            if (enemyType == EnemyType.TowerBeam)
            {
                lineRenderer = GetComponentInChildren<LineRenderer>();
            }
            else if (enemyType == EnemyType.TowerGun)
            {
                // Gun tower should be correct rotation (facing enemy ship).
                turret.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
        }

        private void Update()
        {
            // Make sure this tower isn't current being placed.
            if (!isPlacing)
            {
                // Check for collisions within a radius.
                CheckCollision();
            }
        }

        private void CheckCollision()
        {
            GetComponents(out Collider[] collisions, out Vector3 enemyPos, out EnemyBase enemy);
            // Make sure enemy isn't null.
            if (enemy != null)
            {
                // Do things according to what enemy type this instance is.
                switch (enemyType)
                {
                    case EnemyType.TowerBeam:
                        // Enable the line renderer (laser).
                        LineRenderer(enable: true);
                        // Shoot out the laser at the enemy.
                        Laser(target: enemyPos);
                        break;
                    case EnemyType.TowerGun:
                        // Rotate turret to the enemy.
                        RotateTurret(target: enemyPos);
                        // Instantiate bullets with a delay.
                        ShootBullet(target: enemyPos);
                        break;
                    default:
                        break;
                }
                // Play the shooting audio.
                PlayAudio();
                // Only damage the target with a specific intervals.
                DamageTimer(enemy);
            }
            // If there are no enemies in range...
            else if (collisions.Length <= 0)
            {
                // Reset the "condition" of the tower.
                ResetTower();
            }
        }

        private void ResetTower()
        {
            timer = 0;
            // Make sure audio isn't playing when enemy is not in range.
            audioSource.Stop();
            if (enemyType == EnemyType.TowerBeam)
            {
                // Disable the line renderer.
                LineRenderer(enable: false);
            }
        }

        private void GetComponents(out Collider[] collisions, out Vector3 enemyPos, out EnemyBase enemy)
        {
            // Get an array of collisions in the radius.
            collisions = Physics.OverlapSphere(transform.position, checkRadius, attackableEnemiesLayers);
            // Make sure enemy variables are null at the start.
            enemy = null;
            enemyPos = Vector3.zero;
            // Make sure collisions array isn't empty (there is indeed an object in the radius).
            if (collisions.Length > 0)
            {
                // If multiply enemies in range, target the one with the lowest HP.
                enemy = collisions.OrderBy(h => h.GetComponent<EnemyBase>().Hitpoints).First().GetComponent<EnemyBase>();
                // Cast the enemy interface down to the class and get the object position.
                enemyPos = (enemy as Enemy).transform.position;
                #if UNITY_EDITOR
                Debug.Log($"{gameObject.name}'s target has {enemy.Hitpoints} hitpoints.");
                #endif
            }
        }

        #region Beam Tower
        private void LineRenderer(bool enable)
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = enable;
            }
        }

        private void Laser(Vector3 target)
        {
            // Set laser starting position to the turret.
            lineRenderer.SetPosition(0, turret.position + new Vector3(0, 0.4f, 0));
            // Set the laser ending position to the enemy.
            lineRenderer.SetPosition(1, target);
        }
        #endregion

        #region Gun Tower
        private void RotateTurret(Vector3 target)
        {
            // Get the direction vector of the enemy position.
            Vector3 direction = target - turret.position;
            // Create a quaternion rotation from this direction.
            Quaternion rotation = Quaternion.LookRotation(direction);
            // Smoothly rotate turret towards target.
            turret.rotation = Quaternion.Slerp(turret.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        private void ShootBullet(Vector3 target)
        {
            // Shoot bullet on intervals.
            bulletShootTimer += Time.deltaTime;
#if UNITY_EDITOR
            Debug.Log(Vector3.Dot(turret.forward, (turret.position - target).normalized));
#endif
            // Make sure turret is facing target before shooting.
            if (bulletShootTimer >= bulletShootTime && Vector3.Dot(turret.forward, (turret.position - target).normalized) <= -0.95f)
            {
                bulletShootTimer = 0;
                // Signal that gunshot can be played.
                playGunShot = true;
                // Instantiate bullets at bullet holes.
                InstantiateBullet(atBulletHole: bulletHole1);
                InstantiateBullet(atBulletHole: bulletHole2);
            }
        }

        private void InstantiateBullet(Transform atBulletHole)
        {
            GameObject bulletInstance = Instantiate(bullet);
            // Make initial position the bullet hole.
            bulletInstance.transform.position = atBulletHole.position;
            // Apply velocity forward relative to the bullet hole position.
            bulletInstance.GetComponent<Rigidbody>().velocity = atBulletHole.transform.forward * bulletSpeed;
        }
        #endregion

        private void PlayAudio()
        {
            // Handle audio differently for different enemy types.
            if (enemyType == EnemyType.TowerBeam && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            else if (enemyType == EnemyType.TowerGun && playGunShot)
            {
                playGunShot = false;
                audioSource.Play();
            }
        }

        private void DamageTimer(EnemyBase enemy)
        {
            timer += Time.deltaTime;
            if (timer >= damageTimerAmount)
            {
                timer = 0;
                // Damage the enemy.
                enemy.TakeDamage(Damage);
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position, checkRadius);
        #endif
    } 
}