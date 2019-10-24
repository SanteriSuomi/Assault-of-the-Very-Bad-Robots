using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; set; }

    private Transform pivot;
    private Camera mainCam;

    [SerializeField]
    private float rotateSpeed = 50;
    [SerializeField]
    private float moveSpeed = 50;
    [SerializeField]
    private float pivotStep = 8;

    private int layerMask;

    private Vector3 pivotHitPoint;

    private bool movePivot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        // Layermask for raycast, only check collisions with layers 9 and 11 (walkable and level).
        layerMask = (1 << 9) | (1 << 11);
    }

    private void Start()
    {
        pivot = GameObject.FindGameObjectWithTag("CameraPivot").transform;
        mainCam = GetComponent<Camera>();
        // Set the maincam to the current pivot position and parent it.
        InitializeCameraToPivot();
    }

    private void InitializeCameraToPivot()
    {
        // Position the camera so that the pivot is in the middle of the screen.
        mainCam.transform.position = pivot.position + new Vector3(0, 17.5f, -8.25f);
        mainCam.transform.parent = pivot;
    }

    private void Update()
    {
        // Calculate the deltaTime.
        float delta = Time.deltaTime;

        InputRotate(delta);
        InputZoom(delta);
        // Move the camera pivot according to user input.
        GetMouseHitForPivot();
        // Smoothly move the pivot to the desired position.
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
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask);

            try
            {
                if (hit.collider.gameObject.layer == 9 
                || hit.collider.gameObject.layer == 11) 
                {
                pivotHitPoint = hit.point + new Vector3(0, -0.08f, 0);
                movePivot = true;
                }
            }
            catch (System.Exception e)
            {
                #if UNITY_EDITOR
                Debug.Log($"{nameof(CameraController)}: {e.Message}, hit collider most likely not valid.");
                #endif
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
