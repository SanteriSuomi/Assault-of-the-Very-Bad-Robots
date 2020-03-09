namespace AOTVBR
{
    public class Bullet : ProjectileBase
    {
        protected override void DeactivateDelay() 
            => TowerGunBulletPool.Instance.Return(this);
    } 
}