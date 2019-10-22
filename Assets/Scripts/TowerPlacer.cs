using System.Collections;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject fundsOutText = default;
    private Camera mainCam;
    private GameObject towerPrefab;
    private Renderer[] towerRenderer;
    private Tower towerType;

    private Color[] defaultColor;

    private readonly string levelTag = "Level";

    private int layerMask;

    private bool isPlacing;

    private void Awake()
    {
        // Only check for collisions in layer 0 (default) and layer 11 (level)
        layerMask = (1 << 0) | (1 << 11);
        mainCam = Camera.main;
    }

    public void ActivatePlacer(GameObject tower)
    {
        towerPrefab = Instantiate(tower);
        towerType = towerPrefab.GetComponent<Tower>();
        if (towerType.Cost > PlayManager.Instance.Funds)
        {
            Destroy(towerPrefab);
            print("HELLO");
            StartCoroutine(ShowFundsOutText());
            return;
        }
        towerRenderer = towerPrefab.GetComponentsInChildren<Renderer>();
        defaultColor = new Color[towerRenderer.Length];
        GetDefaultColors();
        isPlacing = true;
    }

    private IEnumerator ShowFundsOutText()
    {
        fundsOutText.SetActive(true);
        yield return new WaitForSeconds(1);
        fundsOutText.SetActive(false);
    }

    private void GetDefaultColors()
    {
        for (int i = 0; i < towerRenderer.Length; i++)
        {
            defaultColor[i] = towerRenderer[i].material.GetColor("_BaseColor");
        }
    }

    private void Update()
    {
        if (isPlacing)
        {
            RaycastHit hit = RaycastGround();
            hit = UpdateTowerPosition(hit);
            hit = ChangeTowerColor(hit);
            PlaceTower(hit);
        }
    }

    private RaycastHit UpdateTowerPosition(RaycastHit hit)
    {
        towerPrefab.transform.position = new Vector3(hit.point.x, hit.point.y + 2.5f, hit.point.z);
        return hit;
    }

    private RaycastHit RaycastGround()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask);
        return hit;
    }

    private void PlaceTower(RaycastHit hit)
    {
        // Only allow placing turrets on level prefabs.
        if (Input.GetMouseButtonDown(0) && hit.collider.CompareTag(levelTag))
        {
            for (int i = 0; i < towerRenderer.Length; i++)
            {
                towerRenderer[i].material.SetColor("_BaseColor", defaultColor[i]);
            }
            towerPrefab.transform.position = hit.point + new Vector3(0, towerPrefab.transform.localScale.y * 1.5f, 0);
            PlayManager.Instance.Funds -= towerType.Cost;
            isPlacing = false;
        }
    }

    private RaycastHit ChangeTowerColor(RaycastHit hit)
    {
        if (hit.collider.CompareTag(levelTag))
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
        foreach (var item in towerRenderer)
        {
            item.material.SetColor("_BaseColor", color);
        }
    }
}
