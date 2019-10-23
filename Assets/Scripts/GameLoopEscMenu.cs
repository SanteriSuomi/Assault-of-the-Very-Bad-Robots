using System.Collections;
using UnityEngine;

public class GameLoopEscMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject escMenuButtons = default;

    private bool isPaused;

    private void Update()
    {
        if (GameState.Instance.GetState() == GameState.GameStates.PlayMap)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
        yield return new WaitForSeconds(0.1f);
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
