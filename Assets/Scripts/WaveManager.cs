using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; set; }

    private bool hasGameStarted;

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
    }

    private void Update()
    {
        if (hasGameStarted)
        {
            StartCoroutine(PlayMapCountdown());
        }
    }

    private IEnumerator PlayMapCountdown()
    {
        yield return new WaitForSeconds(1);
    }
}
