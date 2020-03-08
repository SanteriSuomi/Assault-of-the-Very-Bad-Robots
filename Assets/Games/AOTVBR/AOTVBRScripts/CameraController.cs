using System.Collections;
using UnityEngine;

namespace AOTVBR
{
    public class CameraController : Singleton<CameraController>
    {
        private Transform pivot;
        private Camera mainCamera;
        private Coroutine pivotMoveCoroutine;
        [SerializeField]
        private LayerMask rayLayerMask = default;
        [SerializeField]
        private Vector3 cameraPosRelativeToPivot = new Vector3(0, 17.5f, -8.25f);
        [SerializeField]
        private Vector3 pivotHitPointOffset = new Vector3(0, -0.08f, 0);

        [SerializeField]
        private float rotateSpeed = 50;
        [SerializeField]
        private float moveSpeed = 50;
        [SerializeField]
        private float pivotMoveSpeed = 8;
        [SerializeField]
        private float clampCameraAbove = 5;
        [SerializeField]
        private float clampCameraBelow = 25;
        [SerializeField]
        private float rayMaxDistance = 100;
        [SerializeField]
        private float pivotPlacementMinNormal = 0.5f;

        protected override void Awake()
        {
            mainCamera = GetComponent<Camera>();
            pivot = GameObject.FindGameObjectWithTag("CameraPivot").transform;
            InitializeCameraPivot();
        }

        private void InitializeCameraPivot()
        {
            mainCamera.transform.position = pivot.position + cameraPosRelativeToPivot;
            mainCamera.transform.parent = pivot.transform;
        }

        private void Update()
        {
            Rotate();
            Zoom();
            MovePivot();
        }

        private void Rotate()
        {
            if (Input.GetKey(KeyCode.A))
            {
                RotateAroundAxis(pivot.up);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                RotateAroundAxis(-pivot.up);
            }
        }

        private void RotateAroundAxis(Vector3 axis)
            => transform.RotateAround(pivot.position, axis, rotateSpeed * Time.deltaTime);

        private void Zoom()
        {
            if (Input.GetKey(KeyCode.W) && transform.position.y > clampCameraAbove)
            {
                MoveTowards(moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.S) && transform.position.y < clampCameraBelow)
            {
                MoveTowards(-moveSpeed * Time.deltaTime);
            }
        }

        private void MoveTowards(float distanceDelta)
            => transform.position = Vector3.MoveTowards(transform.position, pivot.position, distanceDelta);

        private void MovePivot()
        {
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit = CastRayFromCamera();
                (bool, Vector3) value = CheckSurfaceNormal(hit);
                if (value.Item1)
                {
                    if (pivotMoveCoroutine != null)
                    {
                        StopCoroutine(pivotMoveCoroutine);
                    }

                    pivotMoveCoroutine = StartCoroutine(SmoothPivotMove(value.Item2));
                }
            }
        }

        private RaycastHit CastRayFromCamera()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance, rayLayerMask);
            return hit;
        }

        private (bool, Vector3) CheckSurfaceNormal(RaycastHit hit)
        {
            if (hit.normal.y > pivotPlacementMinNormal)
            {
                Vector3 newPivotPosition = hit.point + pivotHitPointOffset;
                return (true, newPivotPosition);
            }

            return (false, Vector3.zero);
        }

        private IEnumerator SmoothPivotMove(Vector3 newPivotPosition)
        {
            float maxTimer = 0;
            while (!Utility.PosEqual(pivot.position, newPivotPosition) 
                && maxTimer <= 2.5f)
            {
                maxTimer += Time.deltaTime;
                pivot.position = Vector3.Lerp(pivot.position, newPivotPosition, 
                    pivotMoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}