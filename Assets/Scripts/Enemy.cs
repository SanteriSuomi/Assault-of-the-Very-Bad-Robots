using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
{
    public string Name { get; set; }
    public float Hitpoints { get; set; }
    public int Damage { get; set; }
    public int FundAmount { get; set; }

    [SerializeField]
    private new string name = "Basic Enemy";
    [SerializeField]
    private float hitpoints = 50;
    [SerializeField]
    private int fundAmount = 1;
    [SerializeField]
    private int damage = 5;

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
        if (Hitpoints <= 0.5f)
        {
            Die();
            GiveFunds();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void GiveFunds()
    {
        // Give funds to the player.
        PlayManager.Instance.Funds += fundAmount;
    }
}