using UnityEngine;

namespace AOTVBR
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private float bulletDestroyTime = 3;

        private void OnEnable() 
            => Destroy(gameObject, bulletDestroyTime);
    } 
}