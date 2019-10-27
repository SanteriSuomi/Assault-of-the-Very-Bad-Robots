using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float bulletDestroyTime = 3;

    private void Start()
    {
        // Destroy bullet on delay.
        Destroy(gameObject, bulletDestroyTime);
    }
}
