using UnityEngine;

namespace AOTVBR
{
    public class Bullet : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }
        [SerializeField]
        private float deactivationDelay = 3;

        private void Awake() => Rigidbody = GetComponent<Rigidbody>();

        private void OnEnable() 
            => Invoke(nameof(DeactivateDelay), deactivationDelay);

        private void DeactivateDelay() 
            => TowerGunBulletPool.Instance.Return(this);
    } 
}