using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform pivot;
    private Camera mainCam;
    [SerializeField]
    private float rotateSpeed = 50;
    [SerializeField]
    private float moveSpeed = 50;
    [SerializeField]
    private float pivotStep = 8;

    private Vector3 pivotHitPoint;

    private bool movePivot;

    private void Start()
    {
        pivot = GameObject.FindGameObjectWithTag("CameraPivot").transform;
        mainCam = GetComponent<Camera>();

        InitializePivotPos();
    }

    private void InitializePivotPos()
    {
        mainCam.transform.position = pivot.position + new Vector3(0, 18, -11);
        mainCam.transform.parent = pivot;
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        InputRotate(delta);
        InputZoom(delta);

        GetMouseHitForPivot();
        MovePivot(delta);
    }

    private void InputRotate(float delta)
    {
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.RotateAround(pivot.position, pivot.up, rotateSpeed * delta);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.RotateAround(pivot.position, -pivot.up, rotateSpeed * delta);
        }
    }

    private void InputZoom(float delta)
    {
        if (Input.GetKey(KeyCode.W) && gameObject.transform.position.y > 5)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, pivot.position, moveSpeed * delta);
        }
        else if (Input.GetKey(KeyCode.S) && gameObject.transform.position.y < 30)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, pivot.position, -1 * moveSpeed * delta);
        }
    }

    private void GetMouseHitForPivot()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity);

            if (hit.collider.gameObject.CompareTag("Level") 
                || hit.collider.gameObject.layer == 9 
                || hit.collider.gameObject.layer == 10) 
            {
                pivotHitPoint = hit.point;
                pivotHitPoint.y = RandomLevelGenerator.Instance.YLength;
                movePivot = true;
            }
        }
    }

    private void MovePivot(float delta)
    {
        if (movePivot)
        {
            StartCoroutine(SmoothPivotMove(delta));
        }
    }

    private IEnumerator SmoothPivotMove(float delta)
    {
        float distance = Vector3.Distance(pivot.transform.position, pivotHitPoint);
        pivot.transform.position = Vector3.Lerp(pivot.transform.position, pivotHitPoint, pivotStep * delta);
        yield return new WaitUntil(() => distance <= 0.01f);
        movePivot = false;
    }
}
