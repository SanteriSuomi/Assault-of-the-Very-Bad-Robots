using UnityEngine;
using TMPro;

public class TowerPlacer : MonoBehaviour
{
    private Camera mainCam;
    private GameObject towerPrefab;
    [SerializeField]
    private TextMeshProUGUI placingText;

    private readonly string levelTag = "Level";

    private int layerMask;

    private bool isPlacing;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    public void ActivatePlacer(GameObject tower)
    {
        layerMask = ~(1 << 10);
        isPlacing = true;
        towerPrefab = Instantiate(tower);
    }

    private void Update()
    {
        if (isPlacing)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask);
            
            towerPrefab.transform.position = new Vector3(hit.point.x, hit.point.y + 2.5f, hit.point.z);

            placingText.transform.position = towerPrefab.transform.position + Vector3.right;
            placingText.transform.LookAt(mainCam.transform.position);

            if (Input.GetMouseButtonDown(0))
            {
                if (!hit.collider.CompareTag(levelTag))
                {
                    placingText.transform.position = towerPrefab.transform.position + Vector3.right;
                }
                else
                {
                    towerPrefab.transform.position = hit.point + new Vector3(0, towerPrefab.transform.localScale.y * 1.5f, 0);
                    isPlacing = false;
                }
            }
        }
    }
}
