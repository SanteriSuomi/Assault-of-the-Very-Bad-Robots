using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AOTVBR
{
    #pragma warning disable CA1822 // Can't mark member as static as it's accessed by unity menu
    public class MenuController : Singleton<MenuController>
    {
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
        private WaitForSeconds hideGeneratingTextWFS;

        [SerializeField]
        private float tutorialTextDeactivateMin = 0.2f;
        [SerializeField]
        private float tutorialTextDeactivateMax = 0.3f;
        [SerializeField]
        private float hideGeneratingTextTime = 1;

        protected override void Awake()
        {
            hideGeneratingTextWFS = new WaitForSeconds(hideGeneratingTextTime);
            // Get all the images in the child objects.
            menuButtonImages = menuButtons.GetComponentsInChildren<RawImage>();
            RegisterMenuEvents();
        }

        private void RegisterMenuEvents()
        {
            GameState.Instance.MainMenuEvent += ActivateMainMenu;
            GameState.Instance.GenerateMenuEvent += ActivateGenerateMenu;
            GameState.Instance.PlayMapMenuEvent += ActivatePlayMenu;
            LevelGenerator.Instance.GeneratingTextHideEvent += HideGeneratingText;
        }

        // Register this event in start because awake is too early (for some reason).
        private void Start()
            => PlayStateManager.Instance.GameMenuHideEvent += DisablePlayMenu;

        #region Button & Event Methods
        private void ActivateMainMenu()
        {
            ShowMainMenuButtons();
            HideGenerateButtons();
        }

        private void ActivateGenerateMenu()
        {
            StartCoroutine(FadeOutMainMenu());
            HideMainMenuButtons();
            ShowGenerateButtons();
        }

        private void ActivatePlayMenu()
        {
            HideGenerateButtons();
            ShowTowerButtons();
            ShowHealthFundsText();
        }

        private void DisablePlayMenu()
        {
            HideTowerButtons();
            HideHealthFundsText();
        }

        private void HideGeneratingText() 
            => StartCoroutine(HideGeneratingTextTimer());

        private IEnumerator HideGeneratingTextTimer()
        {
            yield return hideGeneratingTextWFS;
            generatingText.SetActive(false);
        }

        public void ShowGeneratingText() 
            => generatingText.SetActive(true);

        public void ReActivateMenu()
        {
            GameState.Instance.SetState((int)GameStates.GenerateMap);

            Time.timeScale = 1;
            AudioListener.volume = 1;

            HideTowerButtons();
            HideHealthFundsText();
            ShowGenerateButtons();
        }

        public void ExitGame() => Application.Quit();
        #endregion

        #region Private Methods
        private IEnumerator FadeOutMainMenu()
        {
            float alphaMax = 1;
            Color menuBGColor = menuBackground.color;
            Color buttonPlayColor = menuButtonImages[0].color;
            Color buttonExitColor = menuButtonImages[1].color;
            for (float a = alphaMax; a > 0; a -= 1 * Time.deltaTime)
            {
                menuBGColor.a = a;
                menuBackground.color = menuBGColor;
                buttonPlayColor.a = a;
                menuButtonImages[0].color = buttonPlayColor;
                buttonExitColor.a = a;
                menuButtonImages[1].color = buttonExitColor;

                if (a > tutorialTextDeactivateMin 
                    && a < tutorialTextDeactivateMax)
                {
                    tutorialText.SetActive(false);
                }

                yield return null;
            }

            menuBackground.gameObject.SetActive(false);
        }

        private void ShowTowerButtons() 
            => towerButtons.SetActive(true);

        private void HideTowerButtons() 
            => towerButtons.SetActive(false);

        private void ShowHealthFundsText() 
            => healthFundsText.SetActive(true);

        private void HideHealthFundsText() 
            => healthFundsText.SetActive(false);

        private void ShowMainMenuButtons() 
            => menuButtons.SetActive(true);

        private void HideMainMenuButtons() 
            => menuButtons.SetActive(false);

        private void ShowGenerateButtons() 
            => generateButtons.SetActive(true);

        private void HideGenerateButtons() 
            => generateButtons.SetActive(false);
        #endregion
    }
}