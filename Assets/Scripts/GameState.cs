using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
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
    }

    private void Start()
    {
        currentState = GameStates.Menu;
    }

    private enum GameStates
    {
        Menu = 1,
        GenerateMap = 2,
        PlayMap = 3
    }

    private GameStates currentState;

    public void SetState(int state)
    {
        currentState = (GameStates)state;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameStates.Menu:
                ShowMenuButtons(show: true);
                ShowGenerateButtons(show: false);
                break;
            case GameStates.GenerateMap:
                StartCoroutine(FadeOutMenu());
                break;
            case GameStates.PlayMap:
                ShowGenerateButtons(show: false);
                ShowMenuButtons(show: false);
                break;
            default:
                break;
        }
    }

    private IEnumerator FadeOutMenu()
    {
        float alphaMax = 1.0f;
        Color menuBGColor = menuBackground.color;
        Color buttonPlayColor = menuButtonImages[0].color;
        Color buttonExitColor = menuButtonImages[1].color;
        for (float i = alphaMax; i > 0 ; i -= 1f * Time.deltaTime)
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
        ShowMenuButtons(show: false);
        ShowGenerateButtons(show: true);
    }

    private void ShowMenuButtons(bool show)
    {
        menuButtons.SetActive(show);
    }

    private void ShowGenerateButtons(bool show)
    {
        generateButtons.SetActive(show);
    }

    public void ExitGame()
    {
        Application.Quit(0);
    }
}