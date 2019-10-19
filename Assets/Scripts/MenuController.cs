using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private static MenuController Instance { get; set; }

    [SerializeField]
    private RawImage menuBackground = default;
    [SerializeField]
    private GameObject menuButtons = default;
    [SerializeField]
    private GameObject generateButtons = default;

    private Image[] menuButtonImages;

    private void Awake()
    {
        menuButtonImages = menuButtons.GetComponentsInChildren<Image>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #region Event Methods
    public void ActivateMainMenu()
    {
        ShowMenuButtons();
        HideGenerateButtons();
    }

    public void ActivateGenerateMenu()
    {
        StartCoroutine(FadeOutMainMenu());
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
        HideMenuButtons();

        ShowGenerateButtons();
    }

    public void ActivatePlayMenu()
    {
        HideMenuButtons();
        HideGenerateButtons();
    }

    public void ExitGame()
    {
        Application.Quit(0);
    }
    #endregion

    #region Private Button Methods
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
        generateButtons.SetActive(true);
    }
    #endregion
}
