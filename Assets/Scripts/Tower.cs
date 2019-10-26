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

    [SerializeField]
    private float damageTimerAmount = 1;
    [SerializeField]
    private float checkRadius = 3.25f;
    private float timer;
    [SerializeField] [Range(0, 1)]
    private float rotationSpeed = 0.75f;
    [SerializeField]
    private float bulletSpeed = 10;

    int layerMask;

    private bool isPlacing;

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
        if (enemyType == EnemyType.TowerBeam)
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
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

                    GameObject bulletInstance1 = Instantiate(bullet);
                    bulletInstance1.transform.position = bulletHole1.position;
                    bulletInstance1.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * bulletSpeed);

                    GameObject bulletInstance2 = Instantiate(bullet);
                    bulletInstance2.transform.position = bulletHole2.position;
                    bulletInstance2.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * bulletSpeed);
                    break;
                default:
                    break;
            }
            // Only damage the target with a specific intervals.
            DamageTimer(enemy);
        }
        // Check if there is no objects within radius.
        else if (collisions.Length <= 0)
        {
            timer = 0;
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
        Vector3 direction = collisions[0].transform.position - turret.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        turret.rotation = Quaternion.Slerp(turret.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
    #endregion

    private void DealDamage(IEnemy enemy)
    {
        // Subtract hitpoints from the target enemy.
        enemy.Hitpoints -= Damage;

        #if UNITY_EDITOR
        Debug.Log($"Dealt {Damage} to {enemy.Name}, it now has {enemy.Hitpoints} Hitpoints left.");
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