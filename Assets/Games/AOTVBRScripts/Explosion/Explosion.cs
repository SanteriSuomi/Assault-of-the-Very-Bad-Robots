using UnityEngine;

namespace AOTVBR
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField]
        private float deactivateTime = 3;

        private void OnEnable() 
            => Invoke(nameof(Deactivate), deactivateTime);

        private void Deactivate() 
            => ExplosionPool.Instance.Return(this);
    } 
}