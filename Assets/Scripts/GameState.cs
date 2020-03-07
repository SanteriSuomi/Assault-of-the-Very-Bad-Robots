using System.Collections.Generic;
using UnityEngine;

namespace AOTVBR
{
    public enum GameStates
    {
        Menu = 1,
        GenerateMap = 2,
        PlayMap = 3
    }

    public class GameState : Singleton<GameState>
    {
        public delegate void MainMenu();
        public event MainMenu MainMenuEvent;

        public delegate void GenerateMenu();
        public event GenerateMenu GenerateMenuEvent;

        public delegate void PlayMapMenu();
        public event PlayMapMenu PlayMapMenuEvent;

        public delegate void GameStarted();
        public event GameStarted GameStartedEvent;

        private void Start() 
            => currentState = GameStates.Menu;

        private GameStates currentState;

        public void SetState(int state) 
            => currentState = (GameStates)state;

        public GameStates GetState() => currentState;

        private void Update()
        {
            switch (currentState)
            {
                case GameStates.Menu:
                    MainMenuEvent?.Invoke();
                    break;

                case GameStates.GenerateMap:
                    GenerateMenuEvent?.Invoke();
                    ClearEntities();
                    break;

                case GameStates.PlayMap:
                    PlayMapMenuEvent?.Invoke();
                    GameStartedEvent?.Invoke();
                    break;

                default:
                    break;
            }
        }

        private static void ClearEntities()
        {
            List<GameObject> entities = EntityData.Instance.ActiveMapEntityList;
            if (entities.Count > 0)
            {
                for (int i = 0; i < entities.Count - 1; i++)
                {
                    if (entities[i] != null)
                    {
                        Destroy(entities[i]);
                    }
                }
            }
        }
    }
}