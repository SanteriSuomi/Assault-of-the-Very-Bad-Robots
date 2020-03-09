namespace AOTVBR
{
    public class EnemyBasic : EnemyBase
    {
        public override void DeathEvent() 
            => EnemyBasicPool.Instance.Return(this);
    }
}