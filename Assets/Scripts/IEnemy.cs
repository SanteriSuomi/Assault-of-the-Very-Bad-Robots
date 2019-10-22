public interface IEnemy
{
    string Name { get; set; }
    float Hitpoints { get; set; }
    void Die();
}