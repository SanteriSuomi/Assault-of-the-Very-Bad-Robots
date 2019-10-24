using UnityEngine;

public class TowerBeam : MonoBehaviour, ITower
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public float Damage { get; set; }

    [SerializeField]
    private new string name = "Beam Tower";
    [SerializeField]
    private int cost = 10;
    [SerializeField]
    private float damage = 10;

    [SerializeField]
    private float damageTimerAmount = 1;
    private float timer;

    [SerializeField]
    private Transform turret = default;
    private LineRenderer lineRenderer;

    private bool isPlacing;

    private void Awake()
    {
        // Initialize properties with the serialized values.
        Name = name;
        Cost = cost;
        Damage = damage;
    }

    private void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    public void IsPlacing(bool enable)
    {
        // Public method for informing this instance of the tower that it is being placed currently.
        isPlacing = enable;
    }

    private void OnTriggerStay(Collider collision)
    {
        // Make sure the tower doesn't shoot etc if it's being placed.
        if (!isPlacing)
        {
            // When the enemy enters the trigger area, get it's enemy interface.
            IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
            // Make sure it is, indeed, an enemy.
            if (enemy != null)
            {
                // Start showing the line renderer (laser).
                LineRenderer(enable: true);
                // Shoot the laser to the collision target.
                Laser(target: collision);
                // Make sure to damage the enemy on specific intervals.
                timer += Time.deltaTime;
                if (timer >= damageTimerAmount)
                {
                    timer = 0;
                    // Damage the enemy.
                    DealDamage(enemy);
                }
            }
            else
            {
                timer = 0;
                LineRenderer(enable: false);
            }
        }
    }

    private void LineRenderer(bool enable)
    {
        lineRenderer.enabled = enable;
    }

    private void Laser(Collider target)
    {
        // Get the enemy position, and draw the line renderer between the tower and the enemy.
        Vector3 enemyPos = target.transform.position;
        lineRenderer.SetPosition(0, turret.position + new Vector3(0, 0.4f, 0));
        lineRenderer.SetPosition(1, enemyPos);
    }

    private void DealDamage(IEnemy enemy)
    {
        // Subtract hitpoints from the target enemy.
        enemy.Hitpoints -= Damage;

        #if UNITY_EDITOR
        Debug.Log($"Dealt {Damage} to {enemy.Name}, it now has {enemy.Hitpoints} Hitpoints left.");
        #endif
    }
}