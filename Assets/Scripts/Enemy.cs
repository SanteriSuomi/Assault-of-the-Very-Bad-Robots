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
        Name = name;
        Hitpoints = hitpoints;
        FundAmount = fundAmount;
        Damage = damage;
    }

    private void Update()
    {
        if (Hitpoints <= 0.1f)
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
        PlayManager.Instance.Funds += fundAmount;
    }
}