using System.Collections;
using UnityEngine;

public class EscMenu : MonoBehaviour
{
    public static EscMenu Instance { get; set; }

    [SerializeField]
    private GameObject escMenuButtons = default;
    
    private bool isPaused = false;

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
        // Make sure it the game is in the correct state before getting input.
        if (GameState.Instance.GetState() == GameState.GameStates.PlayMap || 
            GameState.Instance.GetState() == GameState.GameStates.GenerateMap)
        {
            // Activate the pause menu.
            if (!isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                ActivateEscMenu(true);
                PauseGame(true);
                PauseAudio(true);
                StartCoroutine(IsPausedDelay());
            }
            // Deactivate the pause menu.
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
        // Introduce a frame delay to prevent accidentally double triggering input.
        yield return null;
        isPaused = true;
    }

    private void PauseGame(bool pause)
    {
        // Pause the game according to the parameter.
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
        // Pause the audio according to the parameter.
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

    public void ActivateEscMenu(bool activate)
    {
        escMenuButtons.SetActive(activate);
    }
}
