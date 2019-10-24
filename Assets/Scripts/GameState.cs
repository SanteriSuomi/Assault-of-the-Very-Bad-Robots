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
        if (EntityData.Instance.ActiveMapEntityList.Count > 0)
        {
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