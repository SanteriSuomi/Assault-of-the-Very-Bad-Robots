using UnityEngine;

public class EnemyBasic : MonoBehaviour, IEnemy
{
    public string Name { get; set; }
    public float Hitpoints { get; set; }

    [SerializeField]
    private new string name = "Basic Enemy";
    [SerializeField]
    private int hitpoints = 50;

    private void Awake()
    {
        Name = name;
        Hitpoints = hitpoints;
    }

    private void Update()
    {
        if (Hitpoints <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}