using System.Collections;
using UnityEngine;

namespace AOTVBR
{
    public class TowerPlacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject fundsOutText = default;
        private Camera mainCam;
        private GameObject towerPrefab;
        private Renderer[] towerRenderer;
        private TowerBase towerType;
        private Color[] defaultColor;

        Vector3 hitPosGrid;

        private readonly string levelTag = "Level";

        private int layerMask;

        private bool isPlacing;

        private void Awake()
        {
            // Only check for collisions in layer 0 (default), layer 10 (tower) and layer 11 (level).
            layerMask = (1 << 0) | (1 << 10) | (1 << 11);
            mainCam = Camera.main;
        }

        public void ActivatePlacer(GameObject tower)
        {
            // This method is activated from the button and initializes the needed properties.
            towerPrefab = Instantiate(tower);
            towerType = towerPrefab.GetComponent<TowerBase>();
            // Check if player has funds.
            CheckCost();
            // Make sure to signal the tower that it is being placed.
            towerType.IsCurrentPlacing(enable: true);
            towerRenderer = towerPrefab.GetComponentsInChildren<Renderer>();
            // Store the default colors of the tower.
            GetDefaultColors();
            // Indicate the script the tower is being placed.
            isPlacing = true;
        }

        private void CheckCost()
        {
            // Destroy the prefab if player doesn't have the needed funds.
            if (towerType.Cost > PlayerManager.Instance.Funds)
            {
                if (towerPrefab != null)
                {
                    Destroy(towerPrefab);
                }
                // Display a funds out text.
                StartCoroutine(NoFundsTextDelay());
            }
        }

        private IEnumerator NoFundsTextDelay()
        {
            fundsOutText.SetActive(true);
            yield return new WaitForSeconds(1);
            fundsOutText.SetActive(false);
        }

        private void GetDefaultColors()
        {
            // Initialize the defaultcolors.
            defaultColor = new Color[towerRenderer.Length];
            for (int i = 0; i < towerRenderer.Length; i++)
            {
                // Loop through the towerRenderers and get the current color and store it in an array.
                if (towerRenderer[i].material.HasProperty("_BaseColor"))
                {
                    defaultColor[i] = towerRenderer[i].material.GetColor("_BaseColor");
                }
            }
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
                    towerType.IsCurrentPlacing(enable: false);
                    Destroy(towerPrefab);
                }
            }
            else
            {
                isPlacing = false;
                towerType.IsCurrentPlacing(enable: false);
                Destroy(towerPrefab);
            }
        }

        private RaycastHit UpdateTowerPosition(RaycastHit hit)
        {
            // Only update the tower placing position on level blocks.
            if (towerPrefab != null && hit.collider.CompareTag(levelTag) && hit.normal.y > 0.5f)
            {
                // Calculate a grid with rounding from the hit point.
                hitPosGrid = new Vector3(Mathf.Round(hit.point.x), hit.point.y + 2, Mathf.Round(hit.point.z));
                // Update the tower to the calculated grid.
                towerPrefab.transform.position = hitPosGrid;
            }
            // Grid should be 0 if no level blocks hit.
            hitPosGrid.y = 0;
            return hit;
        }

        private RaycastHit RaycastGround()
        {
            // Translate mouse position input to a ray.
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            // Use that ray to raycast and store the results.
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask);
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
                towerType.IsCurrentPlacing(enable: false);
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
            if (towerPrefab != null)
            {
                // Place the tower on the surface of the level block aligned with the grid.
                towerPrefab.transform.position = hitPosGrid + new Vector3(0, towerPrefab.transform.localScale.y, 0);
                // Deduct funds from the player.
                PlayerManager.Instance.Funds -= towerType.Cost;
                // Add this tower instance to the entities list to make it easy to find later.
                EntityData.Instance.ActiveMapEntityList.Add(towerPrefab);
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