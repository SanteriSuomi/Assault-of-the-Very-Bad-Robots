using System.Collections;
using UnityEngine;

public class EscMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject escMenuButtons = default;

    private bool isPaused;

    public void HideEscButtons()
    {
        escMenuButtons.SetActive(false);
    }
    
    private void Update()
    {
        if (GameState.Instance.GetState() == GameState.GameStates.PlayMap || GameState.Instance.GetState() == GameState.GameStates.GenerateMap)
        {
            if (!isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                ActivateEscMenu(true);
                PauseGame(true);
                PauseAudio(true);
                StartCoroutine(IsPausedDelay());
            }

            if (isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                ActivateEscMenu(false);
                PauseGame(false);
                PauseAudio(false);
                isPaused = false;
            }
        }
    }

    private IEnumerator IsPausedDelay()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        isPaused = true;
    }

    private void PauseGame(bool pause)
    {
        switch (pause)
        {
            case true:
                Time.timeScale = 0;
                break;
            case false:
                Time.timeScale = 1;
                break;
        }
    }

    private void PauseAudio(bool pause)
    {
        switch (pause)
        {
            case true:
                AudioListener.volume = 0;
                break;
            case false:
                AudioListener.volume = 1;
                break;
        }
    }

    private void ActivateEscMenu(bool activate)
    {
        escMenuButtons.SetActive(activate);
    }
}
