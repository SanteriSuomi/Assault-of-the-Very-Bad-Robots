using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; set; }

    public int Health { get; set; }
    public int Funds { get; set; }

    [SerializeField]
    private TextMeshProUGUI gameStartedCountdownText = default;
    [SerializeField]
    private TextMeshProUGUI healthText = default;
    [SerializeField]
    private TextMeshProUGUI fundsText = default;
    [SerializeField]
    private TextMeshProUGUI timeText = default;
    [SerializeField]
    private GameObject enemyBasic = default;
    [SerializeField]
    private GameObject enemyStrong = default;

    [SerializeField]
    private int gameStartCountdownTime = 3;
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private int funds = 10;

    [SerializeField]
    private float enemyBasicSpawnInterval = 5;
    [SerializeField]
    private float enemyStrongSpawnInterval = 25;

    private float enemyTimerBasic;
    private float enemyTimerStrong;
    private float textTime;

    #region Game Flow Bools
    private bool hasGameStarted = false;
    private bool hasGameResetted = false;
    private bool hasCountdownPlayed = false;
    private bool continueGame = false;
    #endregion

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

        GameState.Instance.GameStartedEvent += GameStart;
    }

    public void GameReset()
    {
        // Reset all the game variables back to zero.
        hasGameStarted = false;
        hasCountdownPlayed = false;
        continueGame = false;
        textTime = 0;
        Funds = funds;
        Health = health;
    }

    private void GameStart()
    {
        if (!hasGameResetted)
        {
            // Make sure to reset the game only at the end.
            hasGameResetted = true;
            // Make sure the game variables have been resetted.
            GameReset();
        }
        // Variable which stores whether the main game has started or not.
        hasGameStarted = true;
    }

    private void Update()
    {
        if (hasGameStarted)
        {
            // Play a countdown text once.
            if (!hasCountdownPlayed)
            {
                hasCountdownPlayed = true;
                StartCoroutine(PlayMapCountdown());
            }
            // Continue the game to the main game loop after the countdown.
            if (continueGame)
            {
                GameLoop();
            }
        }
    }

    private IEnumerator PlayMapCountdown()
    {
        // Make cursor invisible and lock it to the screen.
        CursorLock(false, CursorLockMode.Locked);
        // Countdown text timer.
        for (int i = gameStartCountdownTime; i >= 0; i--)
        {
            gameStartedCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        // Reset the countdown text by making it empty string.
        gameStartedCountdownText.text = string.Empty;
        CursorLock(true, CursorLockMode.None);
        // Signal that we can continue the game.
        continueGame = true;
    }

    private void CursorLock(bool cursorVisible, CursorLockMode cursorLock)
    {
        Cursor.visible = cursorVisible;
        Cursor.lockState = cursorLock;
    }

    private void GameLoop()
    {
        // Reset the game if base health reaches zero or less.
        if (Health <= 0)
        {
            // When dead, reset the game.
            hasGameResetted = false;
            GameReset();
        }
        // Update the health, fund and time info on top of the screen.
        UpdateHealthFundTimeText();
        // Spawn the enemies on intervals.
        SpawnEnemyBasic(enemy: enemyBasic, interval: enemyBasicSpawnInterval);
        SpawnEnemyStrong(enemy: enemyStrong, interval: enemyStrongSpawnInterval);
    }

    private void UpdateHealthFundTimeText()
    {
        healthText.text = $"Health: {Health}";
        fundsText.text = $"Funds: {Funds}";
        // Time text timer.
        textTime += Time.deltaTime;
        timeText.text = $"Time Survived Against the Very Bad Robots: {Mathf.RoundToInt(textTime)}";
    }

    private void SpawnEnemyBasic(GameObject enemy, float interval)
    {
        enemyTimerBasic += Time.deltaTime;
        if (enemyTimerBasic >= interval)
        {
            enemyTimerBasic = 0;
            // Initialize the given parameter gameobject on given intervals and get the navmeshagent.
            InitializeEnemy(enemy, out GameObject spawnedEnemy, out NavMeshAgent enemyAgent);
            // Set the required path for the enemy to travel (leveldata).
            SetEnemyPath(spawnedEnemy, enemyAgent);
        }
    }

    private void SpawnEnemyStrong(GameObject enemy, float interval)
    {
        enemyTimerStrong += Time.deltaTime;
        if (enemyTimerStrong >= interval)
        {
            enemyTimerStrong = 0;
            // Initialize the given parameter gameobject on given intervals and get the navmeshagent.
            InitializeEnemy(enemy, out GameObject spawnedEnemy, out NavMeshAgent enemyAgent);
            // Set the required path for the enemy to travel (leveldata).
            SetEnemyPath(spawnedEnemy, enemyAgent);
        }
    }

    private void InitializeEnemy(GameObject enemy, out GameObject spawnedEnemy, out NavMeshAgent enemyAgent)
    {
        spawnedEnemy = Instantiate(enemy);
        enemyAgent = spawnedEnemy.GetComponent<NavMeshAgent>();
        // Add the enemy isntance to the entitylist.
        EntityData.Instance.ActiveMapEntityList.Add(spawnedEnemy);
    }

    private void SetEnemyPath(GameObject spawnedEnemy, NavMeshAgent enemyAgent)
    {
        // Set the starting point.
        spawnedEnemy.transform.position = LevelData.Instance.AgentStartPoint;
        // Enable navmeshagent after placing it on the navmesh.
        enemyAgent.enabled = true;
        // Start moving to the end point.
        enemyAgent.SetDestination(LevelData.Instance.AgentEndPoint);
    }
}