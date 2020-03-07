using System.Collections;
using UnityEngine;

namespace AOTVBR
{
    public class EscMenu : Singleton<EscMenu>
    {
        [SerializeField]
        private GameObject escMenuButtons = default;

        private bool isPaused;

        private void Update()
        {
            if (GameState.Instance.GetState() == GameStates.PlayMap
                || GameState.Instance.GetState() == GameStates.GenerateMap)
                return;

            EscapeMenuInput();
        }

        private void EscapeMenuInput()
        {
            bool pressedEscape = Input.GetKeyDown(KeyCode.Escape);
            if (isPaused && pressedEscape)
            {
                EnableEscMenu(false);
                PauseGame(false);
                PauseAudio(false);
                isPaused = false;
            }
            else if (pressedEscape)
            {
                EnableEscMenu(true);
                PauseGame(true);
                PauseAudio(true);
                StartCoroutine(IsPauseTrueDelay());
            }
        }

        private IEnumerator IsPauseTrueDelay()
        {
            // Introduce a frame delay to prevent accidentally double triggering input.
            yield return null;
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

        public void EnableEscMenu(bool activate)
            => escMenuButtons.SetActive(activate);
    }
}