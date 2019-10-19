using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; set; }

    [SerializeField]
    private UnityEvent MainMenu = default;
    [SerializeField]
    private UnityEvent GenerateMenu = default;
    [SerializeField]
    private UnityEvent PlayMapMenu = default;

    private void Awake()
    {
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
                MainMenu.Invoke();
                break;
            case GameStates.GenerateMap:
                GenerateMenu.Invoke();
                break;
            case GameStates.PlayMap:
                PlayMapMenu.Invoke();
                break;
            default:
                break;
        }
    }
}