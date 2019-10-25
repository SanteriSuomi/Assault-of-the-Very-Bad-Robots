using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; set; }
    // Events that control some menus in the game using menucontroller.
    #region Menu Events
    public delegate void MainMenu();
    public event MainMenu MainMenuEvent;

    public delegate void GenerateMenu();
    public event GenerateMenu GenerateMenuEvent;

    public delegate void PlayMapMenu();
    public event PlayMapMenu PlayMapMenuEvent;
    #endregion
    // Main game loop has started event.
    public delegate void GameStarted();
    public event GameStarted GameStartedEvent;

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

    private void Start()
    {
        // Start the game with the menu state.
        currentState = GameStates.Menu;
    }

    public enum GameStates
    {
        // All available states in the game.
        Menu = 1,
        GenerateMap = 2,
        PlayMap = 3
    }

    private GameStates currentState;

    public void SetState(int state)
    {
        // Set the state with the underlying integer.
        currentState = (GameStates)state;
    }

    public GameStates GetState()
    {
        // Get the currently running state.
        return currentState;
    }

    private void Update()
    {
        #if UNITY_EDITOR 
        StateDebug(); 
        #endif

        switch (currentState)
        {
            case GameStates.Menu:
                MainMenuEvent.Invoke();

                break;
            case GameStates.GenerateMap:
                GenerateMenuEvent.Invoke();
                // When entering this state, clear all alive entities in the game.
                ClearEntities();

                break;
            case GameStates.PlayMap:
                PlayMapMenuEvent.Invoke();
                GameStartedEvent.Invoke();

                break;
            default:
                break;
        }
    }

    private static void ClearEntities()
    {
        // Check if there is active entities.
        if (EntityData.Instance.ActiveMapEntityList.Count > 0)
        {
            // Destroy every entity in the list.
            foreach (GameObject entity in EntityData.Instance.ActiveMapEntityList)
            {
                Destroy(entity);
            }
        }
    }

    #region State Debug
    #if UNITY_EDITOR
    private void StateDebug()
    {
        switch (currentState)
        {
            case GameStates.Menu:
                Debug.Log("Current State: Menu");
                break;
            case GameStates.GenerateMap:
                Debug.Log("Current State: GenerateMap");
                break;
            case GameStates.PlayMap:
                Debug.Log("Current State: PlayMap");
                break;
            default:
                break;
        }
    }
    #endif
    #endregion
}