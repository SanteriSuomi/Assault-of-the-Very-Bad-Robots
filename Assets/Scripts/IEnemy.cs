public interface IEnemy
{
    string Name { get; set; }
    float Hitpoints { get; set; }
    int Damage { get; set; }
    float FundAmount { get; set; }
    void Die();
}