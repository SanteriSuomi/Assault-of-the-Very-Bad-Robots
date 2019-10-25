﻿using UnityEngine;

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
    private float checkRadius = 3.25f;

    int layerMask;

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
        // Only check for collisions with layer 12 (enemy).
        layerMask = 1 << 12;
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
        // Get an array of collisions in the radius.
        Collider[] collisions = Physics.OverlapSphere(transform.position, checkRadius, layerMask);
        // Make enemy null at the start.
        IEnemy enemy = null;
        // Make sure collisions array isn't empty (there is indeed an object in the radius).
        if (collisions.Length > 0)
        {
            // Get the first enemy.
            enemy = collisions[0].GetComponent<IEnemy>();
        }
        // Make sure enemy isn't null.
        if (enemy != null)
        {
            // Enable the line renderer (laser).
            LineRenderer(enable: true);
            // Shoot out the laser at the enemy.
            Laser(target: collisions[0]);
            // Only damage the target with a specific intervals.
            timer += Time.deltaTime;
            if (timer >= damageTimerAmount)
            {
                timer = 0;
                // Damage the enemy.
                DealDamage(enemy);
            }
        }
        // Check if there is no objects within radius.
        else if (collisions.Length <= 0)
        {
            timer = 0;
            // Disable the line renderer.
            LineRenderer(enable: false);
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
        // Set laser starting position to the turret.
        lineRenderer.SetPosition(0, turret.position + new Vector3(0, 0.4f, 0));
        // Set the laser ending position to the enemy.
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

    private void OnDrawGizmos()
    {
        // Draw a radius sphere for debugging.
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}