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

    private Vector3 pivotHitPoint;

    private int layerMask;

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
        // Initialize the maincam to the current pivot position and parent it.
        InitializeCameraToPivot();
    }

    private void InitializeCameraToPivot()
    {
        // Position the camera so that the pivot is in the middle of the screen.
        mainCam.transform.position = pivot.position + new Vector3(0, 17.5f, -8.25f);
        // Parent camera to the pivot.
        mainCam.transform.parent = pivot;
    }

    private void Update()
    {
        // Calculate the deltaTime.
        float delta = Time.deltaTime;
        // Camera rotation.
        InputRotate(delta);
        // Camera zoom.
        InputZoom(delta);
        // Move the camera pivot according to user input.
        GetMouseHitForPivot();
        // If the hitpoint has been updated.
        if (movePivot)
        {
            // Start a coroutine for smooth movement. 
            StartCoroutine(SmoothPivotMove(delta));
        }
    }

    private void InputRotate(float delta)
    {
        if (Input.GetKey(KeyCode.A))
        {
            // Rotate around the pivot position.
            gameObject.transform.RotateAround(pivot.position, pivot.up, rotateSpeed * delta);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.RotateAround(pivot.position, -pivot.up, rotateSpeed * delta);
        }
    }

    private void InputZoom(float delta)
    {
        // Make sure to clamp the Y position to above 5 && below 30.
        if (Input.GetKey(KeyCode.W) && gameObject.transform.position.y > 5)
        {
            // Rotate towards the pivot position.
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
            // Get a ray from mouse position to the world position.
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            // Raycast that ray, and get the hit data.
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask);

            try
            {
                // Only collide with layers 9 (walkable) and 11 (level).
                if (hit.collider.gameObject.layer == 9 
                || hit.collider.gameObject.layer == 11) 
                {
                    // Pivot hitpoint is the raycast hitpoint.
                    pivotHitPoint = hit.point + new Vector3(0, -0.08f, 0);
                    // Indicate that hitpoint has been updated.
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

    private IEnumerator SmoothPivotMove(float delta)
    {
        // Get the distance between current pivot position and the new hit position.
        float distance = Vector3.Distance(pivot.transform.position, pivotHitPoint);
        // Smoothly lerp the pivot to the position.
        pivot.transform.position = Vector3.Lerp(pivot.transform.position, pivotHitPoint, pivotStep * delta);
        // Stop the coroutine when the current distance is close to the desired position.
        yield return new WaitUntil(() => Mathf.Approximately(distance, Mathf.Epsilon));
        // Indicate that we aren't moving anymore.
        movePivot = false;
    }
}
