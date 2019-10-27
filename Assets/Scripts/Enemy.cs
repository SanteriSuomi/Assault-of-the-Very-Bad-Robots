using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
{
    public string Name { get; set; }
    public float Hitpoints { get; set; }
    public int Damage { get; set; }
    public float FundAmount { get; set; }

    [SerializeField]
    private new string name = "Enemy";
    [SerializeField]
    private float hitpoints = 50;
    [SerializeField]
    private float fundAmount = 1;
    [SerializeField]
    private int damage = 5;
    [SerializeField]
    private GameObject explosionPrefab = default;

    private void Awake()
    {
        // Apply the serialized values for this enemy instance.
        Name = name;
        Hitpoints = hitpoints;
        Damage = damage;
        FundAmount = fundAmount;
    }

    private void Update()
    {
        // Check if hitpoints are close or less than zero.
        if (Mathf.Approximately(Hitpoints, Mathf.Epsilon) || Hitpoints <= Mathf.Epsilon)
        {
            // Start death process.
            Die();
            // Give funds to the player.
            GiveFunds();
        }
    }

    public void Die()
    {
        // Start an explosion.
        Explosion();
        Destroy(gameObject);
    }

    private void Explosion()
    {
        Instantiate(explosionPrefab).transform.position = transform.position;
    }

    private void GiveFunds()
    {
        PlayManager.Instance.Funds += fundAmount;
    }
}