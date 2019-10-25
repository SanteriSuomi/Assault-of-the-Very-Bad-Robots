public interface IEnemy
{
    string Name { get; set; }
    float Hitpoints { get; set; }
    int Damage { get; set; }
    int FundAmount { get; set; }
    void Die();
}