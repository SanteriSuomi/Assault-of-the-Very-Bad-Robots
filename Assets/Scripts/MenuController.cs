using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; set; }

    // Various menu elements.
    [SerializeField]
    private RawImage menuBackground = default;
    [SerializeField]
    private GameObject menuButtons = default;
    [SerializeField]
    private GameObject tutorialText = default;
    [SerializeField]
    private GameObject generateButtons = default;
    [SerializeField]
    private GameObject generatingText = default;
    [SerializeField]
    private GameObject towerButtons = default;
    [SerializeField]
    private GameObject healthFundsText = default;
    private RawImage[] menuButtonImages;

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
        // Get all the images in the child objects.
        menuButtonImages = menuButtons.GetComponentsInChildren<RawImage>();
        RegisterAwakeEvents();
    }

    private void RegisterAwakeEvents()
    {
        // Register events from other scripts.
        GameState.Instance.MainMenuEvent += ActivateMainMenu;
        GameState.Instance.GenerateMenuEvent += ActivateGenerateMenu;
        GameState.Instance.PlayMapMenuEvent += ActivatePlayMenu;
        LevelGenerator.Instance.GeneratingTextHideEvent += HideGeneratingText;
    }

    private void Start()
    {
        // Register this event in start because awake is too early (for some reason).
        PlayManager.Instance.GameMenuHideEvent += DisablePlayMenu;
    }

    #region Button & Event Methods
    private void ActivateMainMenu()
    {
        // Main method for activating main menu, through and event.
        ShowMenuButtons();
        HideGenerateButtons();
    }

    private void ActivateGenerateMenu()
    {
        // Main method for activating generating menu, through and event.
        // Fade main menu elements when activating generation menu.
        StartCoroutine(FadeOutMainMenu());
        HideMenuButtons();
        ShowGenerateButtons();
    }

    private IEnumerator FadeOutMainMenu()
    {
        // Alpha must be 1.0f at the beginning.
        float alphaMax = 1.0f;
        GetColoComponents(out Color menuBGColor, out Color buttonPlayColor, out Color buttonExitColor);
        // Gradually go down with a for loop.
        for (float currentAlpha = alphaMax; currentAlpha > 0; currentAlpha -= 1f * Time.deltaTime)
        {
            // Change the color component's alpha.
            ChangeAlpha(ref menuBGColor, ref buttonPlayColor, ref buttonExitColor, currentAlpha);
            // Deactivate the tutorial text between values.
            DeactivateTutorialText(currentAlpha);
            yield return null;
        }
        // Deactivate the background at the end.
        menuBackground.gameObject.SetActive(false);
    }

    private void GetColoComponents(out Color menuBGColor, out Color buttonPlayColor, out Color buttonExitColor)
    {
        // Get all the color data from the menu elements.
        menuBGColor = menuBackground.color;
        buttonPlayColor = menuButtonImages[0].color;
        buttonExitColor = menuButtonImages[1].color;
    }

    private void ChangeAlpha(ref Color menuBGColor, ref Color buttonPlayColor, ref Color buttonExitColor, float currentAlpha)
    {
        // Assign the new alpha values.
        menuBGColor.a = currentAlpha;
        menuBackground.color = menuBGColor;
        buttonPlayColor.a = currentAlpha;
        menuButtonImages[0].color = buttonPlayColor;
        buttonExitColor.a = currentAlpha;
        menuButtonImages[1].color = buttonExitColor;
    }

    private void DeactivateTutorialText(float i)
    {
        // Deactivate the tutorial text inbetween.
        if (i > 0.2f && i < 0.3f)
        {
            tutorialText.SetActive(false);
        }
    }

    private void ActivatePlayMenu()
    {
        // Main method for activating the main game menu.
        HideGenerateButtons();
        ShowTowerButtons();
        ShowHealthFundsText();
    }

    public void ReActivateMenu()
    {
        // Method for showing the menu again (from a button in the pause menu).
        // If the current state isn't the correct one already, set it.
        if (GameState.Instance.GetState() != GameState.GameStates.GenerateMap )
        {
            GameState.Instance.SetState(2);
        }
        // Make sure game isn't paused or muted, as it's exiting the pause menu.
        Time.timeScale = 1;
        AudioListener.volume = 1;
        // Hide/show the required menu elements.
        HideTowerButtons();
        HideHealthFundsText();
        ShowGenerateButtons();
    }
    
    private void DisablePlayMenu()
    {
        HideTowerButtons();
        HideHealthFundsText();
    }
    
    public void ShowGeneratingText()
    {
        generatingText.SetActive(true);
    }

    private void HideGeneratingText()
    {
        StartCoroutine(HideGeneratingTextTimer());
    }

    private IEnumerator HideGeneratingTextTimer()
    {
        // Small delay until deactivating the generating text.
        yield return new WaitForSeconds(1);
        generatingText.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit(0);
    }
    #endregion
    // Single methods for showing/hiding specific UI elements.
    #region Private Methods
    private void ShowTowerButtons()
    {
        towerButtons.SetActive(true);
    }

    private void HideTowerButtons()
    {
        towerButtons.SetActive(false);
    }

    private void ShowHealthFundsText()
    {
        healthFundsText.SetActive(true);
    }

    private void HideHealthFundsText()
    {
        healthFundsText.SetActive(false);
    }

    private void ShowMenuButtons()
    {
        menuButtons.SetActive(true);
    }

    private void HideMenuButtons()
    {
        menuButtons.SetActive(false);
    }

    private void ShowGenerateButtons()
    {
        generateButtons.SetActive(true);
    }

    private void HideGenerateButtons()
    {
        generateButtons.SetActive(false);
    }
    #endregion
}