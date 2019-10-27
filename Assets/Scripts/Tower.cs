using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour, ITower
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public float Damage { get; set; }

    private enum EnemyType
    {
        TowerBeam,
        TowerGun
    }

    [SerializeField]
    private EnemyType enemyType = default;

    [SerializeField]
    private new string name = "Tower";
    [SerializeField]
    private int cost = 10;
    [SerializeField]
    private float damage = 10;

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

    int layerMask;

    private bool isPlacing;
    private bool playGunShot;

    private void Awake()
    {
        // Initialize properties with the serialized values.
        Name = name;
        Cost = cost;
        Damage = damage;
        // Only check for collisions with layer 12 (enemy).
        layerMask = 1 << 12;
    }

    private void Start()
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

    public void IsPlacing(bool enable)
    {
        // Public method for informing this instance of the tower that it is being placed currently.
        isPlacing = enable;
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
        GetComponents(out Collider[] collisions, out IEnemy enemy);
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
                    Laser(target: collisions[0]);
                    break;
                case EnemyType.TowerGun:
                    // Rotate turret to the enemy.
                    RotateTurret(collisions);
                    // Instantiate bullets with a delay.
                    ShootBullet(collisions);
                    break;
                default:
                    break;
            }
            // Play the shooting audio.
            PlayAudio();
            // Only damage the target with a specific intervals.
            DamageTimer(enemy);
        }
        // Check if there is no objects within radius.
        else if (collisions.Length <= 0)
        {
            timer = 0;
            // Make sure audio isn't playing when enemy is not in range.
            audioSource.Stop();
            // Disable the line renderer.
            LineRenderer(enable: false);
        }
    }

    
    private void GetComponents(out Collider[] collisions, out IEnemy enemy)
    {
        // Get an array of collisions in the radius.
        collisions = Physics.OverlapSphere(transform.position, checkRadius, layerMask);
        // Make enemy is null at the start.
        enemy = null;
        // Make sure collisions array isn't empty (there is indeed an object in the radius).
        if (collisions.Length > 0)
        {
            // Get the first enemy collision.
            enemy = collisions[0].GetComponent<IEnemy>();
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

    private void Laser(Collider target)
    {
        // Get the enemy position in the sphere.
        Vector3 enemyPos = target.transform.position;
        // Set laser starting position to the turret.
        lineRenderer.SetPosition(0, turret.position + new Vector3(0, 0.4f, 0));
        // Set the laser ending position to the enemy.
        lineRenderer.SetPosition(1, enemyPos);
    }
    #endregion

    #region Gun Tower
    private void RotateTurret(Collider[] collisions)
    {
        // Get the direction vector of the enemy position.
        Vector3 direction = collisions[0].transform.position - turret.position;
        // Create a quaternion rotation from this direction.
        Quaternion rotation = Quaternion.LookRotation(direction);
        // Smoothly rotate turret towards target.
        turret.rotation = Quaternion.Slerp(turret.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private void ShootBullet(Collider[] collisions)
    {
        // Shoot bullet on intervals.
        bulletShootTimer += Time.deltaTime;
        #if UNITY_EDITOR
        Debug.Log(Vector3.Dot(turret.forward, (turret.position - collisions[0].transform.position).normalized));
        #endif
        // Make sure turret is facing target before shooting.
        if (bulletShootTimer >= bulletShootTime && Vector3.Dot(turret.forward, (turret.position - collisions[0].transform.position).normalized) <= -0.95f)
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

    private void DamageTimer(IEnemy enemy)
    {
        timer += Time.deltaTime;
        if (timer >= damageTimerAmount)
        {
            timer = 0;
            // Damage the enemy.
            DealDamage(enemy);
        }
    }

    private void DealDamage(IEnemy enemy)
    {
        // Subtract hitpoints from the target enemy.
        enemy.Hitpoints -= Damage;

        #if UNITY_EDITOR
        Debug.Log($"{gameObject.name} dealt {Damage} damage to {enemy.Name}, it now has {enemy.Hitpoints} Hitpoints left.");
        #endif
    }

      #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw a radius sphere for debugging.
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
    #endif
}