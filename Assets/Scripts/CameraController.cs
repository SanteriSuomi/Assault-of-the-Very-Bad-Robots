using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform pivot;
    [SerializeField]
    private float rotateSpeed = 50;
    [SerializeField]
    private float moveSpeed = 50;

    private void Start()
    {
        pivot = GameObject.Find("PRE_CameraPivotGround(Clone)").transform;
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.RotateAround(pivot.position, pivot.up, rotateSpeed * delta);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.RotateAround(pivot.position, -pivot.up, rotateSpeed * delta);
        }

        if (Input.GetKey(KeyCode.W) && gameObject.transform.position.y > 5)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, pivot.position, moveSpeed * delta);
        }
        else if (Input.GetKey(KeyCode.S) && gameObject.transform.position.y < 30)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, pivot.position, -1 * moveSpeed * delta);
        }
    }
}
