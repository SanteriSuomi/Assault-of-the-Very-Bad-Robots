namespace AOTVBR
{
    public class EnemyStrong : EnemyBase
    {
        public override void DeathEvent()
            => EnemyStrongPool.Instance.Return(this);
    }
}