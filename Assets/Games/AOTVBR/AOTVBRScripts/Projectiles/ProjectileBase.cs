using UnityEngine;

namespace AOTVBR
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }
        [SerializeField]
        private float deactivationDelay = 3;

        protected virtual void Awake() 
            => Rigidbody = GetComponent<Rigidbody>();

        private void OnEnable()
            => Invoke(nameof(DeactivateDelay), deactivationDelay);

        protected abstract void DeactivateDelay();

        protected virtual void OnCollisionEnter(Collision collision){}
    }
}