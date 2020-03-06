namespace AOTVBR
{
    public interface IDamageable
    {
        IHasName GetIHasName();

        float Hitpoints { get; set; }
        void TakeDamage(float damage);
        void Die();
    }
}