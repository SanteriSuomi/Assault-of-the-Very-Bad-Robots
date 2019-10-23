using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; set; }

    #region Menu Events
    public delegate void MainMenu();
    public event MainMenu MainMenuEvent;

    public delegate void GenerateMenu();
    public event GenerateMenu GenerateMenuEvent;

    public delegate void PlayMapMenu();
    public event PlayMapMenu PlayMapMenuEvent;
    #endregion

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
        currentState = GameStates.Menu;
    }

    public enum GameStates
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

    public GameStates GetState()
    {
        return currentState;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameStates.Menu:
                MainMenuEvent.Invoke();

                #if UNITY_EDITOR
                Debug.Log("Current State: Menu");
                #endif
                break;
            case GameStates.GenerateMap:
                GenerateMenuEvent.Invoke();

                #if UNITY_EDITOR
                Debug.Log("Current State: GenerateMap");
                #endif
                break;
            case GameStates.PlayMap:
                PlayMapMenuEvent.Invoke();
                GameStartedEvent.Invoke();

                #if UNITY_EDITOR
                Debug.Log("Current State: PlayMap");
                #endif
                break;
            default:
                break;
        }
    }
}