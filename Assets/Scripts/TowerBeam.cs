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
        isPlacing = enable;
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!isPlacing)
        {
            IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
            if (enemy != null)
            {
                LineRenderer(enable: true);
                Laser(collision);

                timer += Time.deltaTime;
                if (timer >= damageTimerAmount)
                {
                    timer = 0;
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

    private void Laser(Collider collision)
    {
        Vector3 enemyPos = collision.transform.position;
        lineRenderer.SetPosition(0, turret.position + new Vector3(0, 0.4f, 0));
        lineRenderer.SetPosition(1, enemyPos);
    }

    private void DealDamage(IEnemy enemy)
    {
        enemy.Hitpoints -= Damage;

        #if UNITY_EDITOR
        Debug.Log($"Dealt {Damage} to {enemy.Name}, it now has {enemy.Hitpoints} Hitpoints left.");
        #endif
    }
}