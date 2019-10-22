using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; set; }

    [SerializeField]
    private RawImage menuBackground = default;
    [SerializeField]
    private GameObject menuButtons = default;
    [SerializeField]
    private GameObject generateButtons = default;
    [SerializeField]
    private GameObject generatingText = default;
    [SerializeField]
    private GameObject towerButtons = default;
    [SerializeField]
    private GameObject healthFundsText = default;
    private Image[] menuButtonImages;

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

        menuButtonImages = menuButtons.GetComponentsInChildren<Image>();

        GameState.Instance.MainMenuEvent += ActivateMainMenu;
        GameState.Instance.GenerateMenuEvent += ActivateGenerateMenu;
        GameState.Instance.PlayMapMenuEvent += ActivatePlayMenu;
        RandomLevelGenerator.Instance.GeneratingTextHideEvent += HideGeneratingText;
    }

    #region Public/Event Methods
    private void ActivateMainMenu()
    {
        ShowMenuButtons();
        HideGenerateButtons();
    }

    private void ActivateGenerateMenu()
    {
        StartCoroutine(FadeOutMainMenu());
        HideMenuButtons();
        ShowGenerateButtons();
    }

    private IEnumerator FadeOutMainMenu()
    {
        float alphaMax = 1.0f;
        Color menuBGColor = menuBackground.color;
        Color buttonPlayColor = menuButtonImages[0].color;
        Color buttonExitColor = menuButtonImages[1].color;
        for (float i = alphaMax; i > 0; i -= 1f * Time.deltaTime)
        {
            menuBGColor.a = i;
            menuBackground.color = menuBGColor;
            buttonPlayColor.a = i;
            menuButtonImages[0].color = buttonPlayColor;
            buttonExitColor.a = i;
            menuButtonImages[1].color = buttonExitColor;

            yield return null;
        }

        menuBackground.gameObject.SetActive(false);
    }

    private void ActivatePlayMenu()
    {
        HideGenerateButtons();
        ShowTowerButtons();
        ShowHealthFundsText();
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
        yield return new WaitForSeconds(1);
        generatingText.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit(0);
    }
    #endregion

    #region Private Button Methods
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
