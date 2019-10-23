using UnityEngine;

public class EnemyBasic : MonoBehaviour, IEnemy
{
    public string Name { get; set; }
    public float Hitpoints { get; set; }
    public int FundAmount { get; set; }

    [SerializeField]
    private new string name = "Basic Enemy";
    [SerializeField]
    private float hitpoints = 50;
    [SerializeField]
    private int fundAmount = 1;

    private void Awake()
    {
        Name = name;
        Hitpoints = hitpoints;
        FundAmount = fundAmount;
    }

    private void Update()
    {
        if (Hitpoints <= 0)
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
        GameLoopManager.Instance.Funds += fundAmount;
    }
}