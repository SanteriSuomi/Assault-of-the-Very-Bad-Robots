using System.Collections;
using UnityEngine;

namespace AOTVBR
{
    public class TowerPlacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject fundsOutText = default;
        private Camera mainCamera;
        private GameObject currentTower;
        private TowerBase currentTowerType;
        private Renderer[] towerRenderer;
        private WaitForSeconds outOfFundsWFS;

        [SerializeField]
        private LayerMask towerPlaceRaycastMask = default;
        private Color[] defaultColor;
        private Vector3 hitPosGrid;

        [SerializeField]
        private float outOfFundsTextTime = 1;
        [SerializeField]
        private float towerPlaceRaycastLength = 500;
        [SerializeField]
        private float towerPosUpdateNormalMinAngle = 0.5f;
        [SerializeField]
        private float towerUpdatePosYOffset = 2;
        [SerializeField]
        private float bombTowerPlacePositionYOffsetFix = 0.35f;

        [SerializeField]
        private string towerRendererColorTag = "_BaseColor";
        [SerializeField]
        private string levelTag = "Level";

        private bool isPlacing;

        private void Awake()
        {
            outOfFundsWFS = new WaitForSeconds(outOfFundsTextTime);
            mainCamera = Camera.main;
        }

        public void ActivatePlacer(GameObject tower)
        {
            currentTower = Instantiate(tower);
            currentTowerType = currentTower.GetComponent<TowerBase>();

            if (CheckFunds())
            {
                currentTowerType.IsCurrentPlacing(value: true);
                towerRenderer = currentTower.GetComponentsInChildren<Renderer>();
                SaveDefaultRendererColors();
                isPlacing = true;
            }
        }

        private bool CheckFunds()
        {
            if (currentTowerType.Cost > PlayerData.Instance.Funds)
            {
                if (currentTower != null)
                {
                    Destroy(currentTower);
                }

                StartCoroutine(OutOfFundsText());
                return false;
            }

            return true;
        }

        private void SaveDefaultRendererColors()
        {
            defaultColor = new Color[towerRenderer.Length];
            for (int i = 0; i < towerRenderer.Length; i++)
            {
                if (towerRenderer[i].material.HasProperty(towerRendererColorTag))
                {
                    defaultColor[i] = towerRenderer[i].material.GetColor(towerRendererColorTag);
                }
            }
        }

        private IEnumerator OutOfFundsText()
        {
            fundsOutText.SetActive(true);
            yield return outOfFundsWFS;
            fundsOutText.SetActive(false);
        }

        private void Update()
        {
            if (isPlacing && currentTower != null)
            {
                CheckCancelInput();
                RaycastHit hit = RaycastGround();
                UpdateTowerPosition(hit);
                PlaceTower(hit);
            }
        }

        private void CheckCancelInput()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                isPlacing = false;
                currentTowerType.IsCurrentPlacing(value: false);
                Destroy(currentTower);
            }
        }

        private RaycastHit RaycastGround()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, towerPlaceRaycastLength, towerPlaceRaycastMask);
            return hit;
        }

        private void UpdateTowerPosition(RaycastHit hit)
        {
            if (IsLegalHit(hit))
            {
                hitPosGrid = new Vector3(Mathf.Round(hit.point.x),
                    hit.point.y + towerUpdatePosYOffset,
                    Mathf.Round(hit.point.z));
                currentTower.transform.position = hitPosGrid;
                SetTowerColor(Color.green);
            }
            else
            {
                SetTowerColor(Color.red);
            }

            hitPosGrid.y = 0;
        }

        private void SetTowerColor(Color color)
        {
            for (int i = 0; i < towerRenderer.Length; i++)
            {
                if (towerRenderer[i] != null)
                {
                    towerRenderer[i].material.SetColor(towerRendererColorTag, color);
                }
            }
        }

        private void PlaceTower(RaycastHit hit)
        {
            if (Input.GetMouseButtonDown(0)
                && IsLegalHit(hit))
            {
                SetTowerDefaultColors();
                InitializeTower();
                isPlacing = false;
                currentTowerType.IsCurrentPlacing(value: false);
            }
        }

        private void SetTowerDefaultColors()
        {
            for (int i = 0; i < towerRenderer.Length; i++)
            {
                if (towerRenderer[i] != null
                    && towerRenderer[i].material.HasProperty(towerRendererColorTag))
                {
                    towerRenderer[i].material.SetColor(towerRendererColorTag, defaultColor[i]);
                }
            }
        }

        private void InitializeTower()
        {
            if (currentTowerType is TowerBomb) // Fix Bomb Tower going in to the ground
            {
                currentTower.transform.position = hitPosGrid + new Vector3(0, currentTower.transform.localScale.y 
                    + bombTowerPlacePositionYOffsetFix, 0);
            }
            else
            {
                currentTower.transform.position = hitPosGrid + new Vector3(0, currentTower.transform.localScale.y, 0);
            }
            
            PlayerData.Instance.Funds -= currentTowerType.Cost;
            EntityData.Instance.ActiveMapEntityList.Add(currentTower);
        }

        private bool IsLegalHit(RaycastHit hit)
        {
            return hit.collider.CompareTag(levelTag)
                && hit.normal.y > towerPosUpdateNormalMinAngle;
        }
    }
}