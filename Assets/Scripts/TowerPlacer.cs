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
                currentTowerType.IsCurrentPlacing(enable: true);
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
            for (int i = 0; i < towerRenderer.Length - 1; i++)
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
            // Make sure tower is being placed.
            if (isPlacing)
            {
                // Check if placing should be cancelled.
                CheckPlacerCancel(check: true);
                // Raycast to the ground and store the result.
                RaycastHit hit = RaycastGround();
                // Update the tower position to the hit.
                hit = UpdateTowerPosition(hit);
                // Change tower color based on the hit collider.
                hit = ChangeTowerColor(hit);
                // Place the tower.
                PlaceTower(hit);
            }
        }

        private void CheckPlacerCancel(bool check)
        {
            // If check, check with input, else cancel anyway.
            if (check)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    isPlacing = false;
                    currentTowerType.IsCurrentPlacing(enable: false);
                    Destroy(currentTower);
                }
            }
            else
            {
                isPlacing = false;
                currentTowerType.IsCurrentPlacing(enable: false);
                Destroy(currentTower);
            }
        }

        private RaycastHit UpdateTowerPosition(RaycastHit hit)
        {
            // Only update the tower placing position on level blocks.
            if (currentTower != null && hit.collider.CompareTag(levelTag) && hit.normal.y > 0.5f)
            {
                // Calculate a grid with rounding from the hit point.
                hitPosGrid = new Vector3(Mathf.Round(hit.point.x), hit.point.y + 2, Mathf.Round(hit.point.z));
                // Update the tower to the calculated grid.
                currentTower.transform.position = hitPosGrid;
            }
            // Grid should be 0 if no level blocks hit.
            hitPosGrid.y = 0;
            return hit;
        }

        private RaycastHit RaycastGround()
        {
            // Translate mouse position input to a ray.
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            // Use that ray to raycast and store the results.
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, towerPlaceRaycastMask);
            return hit;
        }

        private void PlaceTower(RaycastHit hit)
        {
            // Only allow placing turrets on level blocks.
            if (Input.GetMouseButtonDown(0) && hit.collider.CompareTag(levelTag) && hit.normal.y > 0.5f)
            {
                // When tower is placed, set it's color to default.
                SetDefaultColor();
                // Position the tower on the block correctly. 
                PositionTower();
                // Make sure placing bools are resetted.
                isPlacing = false;
                currentTowerType.IsCurrentPlacing(enable: false);
            }
        }

        private void SetDefaultColor()
        {
            for (int i = 0; i < towerRenderer.Length; i++)
            {
                if (towerRenderer[i] != null && towerRenderer[i].material.HasProperty("_BaseColor"))
                {
                    // Loop through the towerRenderers and set temporary colors with it's default colors.
                    towerRenderer[i].material.SetColor("_BaseColor", defaultColor[i]);
                }
            }
        }

        private void PositionTower()
        {
            if (currentTower != null)
            {
                // Place the tower on the surface of the level block aligned with the grid.
                currentTower.transform.position = hitPosGrid + new Vector3(0, currentTower.transform.localScale.y, 0);
                // Deduct funds from the player.
                PlayerData.Instance.Funds -= currentTowerType.Cost;
                // Add this tower instance to the entities list to make it easy to find later.
                EntityData.Instance.ActiveMapEntityList.Add(currentTower);
            }
        }

        private RaycastHit ChangeTowerColor(RaycastHit hit)
        {
            // Set colors on the tower depending hit collider.
            if (hit.collider.CompareTag(levelTag) && hit.normal.y > 0.5f)
            {
                SetColor(Color.green);
            }
            else
            {
                SetColor(Color.red);
            }

            return hit;
        }

        private void SetColor(Color color)
        {
            // Method for settings the colors on the towerRenderer array.
            foreach (Renderer renderer in towerRenderer)
            {
                if (renderer != null)
                {
                    renderer.material.SetColor("_BaseColor", color);
                }
            }
        }
    }
}