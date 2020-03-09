namespace AOTVBR
{
    public class Bullet : ProjectileBase
    {
        protected override void DeactivateDelay() 
            => BulletPool.Instance.Return(this);
    } 
}