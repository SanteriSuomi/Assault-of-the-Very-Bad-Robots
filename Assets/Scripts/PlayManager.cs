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

    private float enemyBasicTimer;
    private float enemyStrongTimer;
    private float textTime;

    #region Game Flow Bools
    private bool hasGameStarted = false;
    private bool setHealth = false;
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
        setHealth = false;
        hasCountdownPlayed = false;
        continueGame = false;
        textTime = 0;
        Funds = funds;
        Health = health;
    }

    private void GameStart()
    {
        // Variable which stores whether the main game has started or not.
        hasGameStarted = true;
        // Only set the health and other required variables once.
        if (!setHealth)
        {
            setHealth = true;
            Health = health;
            Funds = funds;
            textTime = 0;
        }
    }

    private void Update()
    {
        if (hasGameStarted)
        {
            if (!hasCountdownPlayed)
            {
                hasCountdownPlayed = true;
                StartCoroutine(PlayMapCountdown());
            }

            if (continueGame)
            {
                GameLoop();
            }
        }
    }

    private IEnumerator PlayMapCountdown()
    {
        CursorLock(false, CursorLockMode.Locked);

        for (int i = gameStartCountdownTime; i >= 0; i--)
        {
            gameStartedCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        gameStartedCountdownText.text = string.Empty;
        CursorLock(true, CursorLockMode.None);

        continueGame = true;
    }

    private void CursorLock(bool cursorVisible, CursorLockMode cursorLock)
    {
        Cursor.visible = cursorVisible;
        Cursor.lockState = cursorLock;
    }

    private void GameLoop()
    {
        if (Health < 0)
        {
            GameReset();
        }

        UpdateHealthFundTimeText();

        SpawnEnemyBasic(enemyBasic, 5);
        SpawnEnemyStrong(enemyStrong, 25);
    }

    private void UpdateHealthFundTimeText()
    {
        healthText.text = $"Health: {Health}";
        fundsText.text = $"Funds: {Funds}";
        textTime += Time.deltaTime;
        timeText.text = $"Time Survived Against the Very Bad Robots: {Mathf.RoundToInt(textTime)}";
    }

    private void SpawnEnemyBasic(GameObject enemy, float time)
    {
        enemyBasicTimer += Time.deltaTime;
        if (enemyBasicTimer >= time)
        {
            enemyBasicTimer = 0;

            InitializeEnemy(enemy, out GameObject spawnedEnemy, out NavMeshAgent enemyAgent);
            SetEnemyPath(spawnedEnemy, enemyAgent);
        }
    }

    private void SpawnEnemyStrong(GameObject enemy, float time)
    {
        enemyStrongTimer += Time.deltaTime;
        if (enemyStrongTimer >= time)
        {
            enemyStrongTimer = 0;

            InitializeEnemy(enemy, out GameObject spawnedEnemy, out NavMeshAgent enemyAgent);
            SetEnemyPath(spawnedEnemy, enemyAgent);
        }
    }

    private void InitializeEnemy(GameObject enemy, out GameObject spawnedEnemy, out NavMeshAgent enemyAgent)
    {
        spawnedEnemy = Instantiate(enemy);
        enemyAgent = spawnedEnemy.GetComponent<NavMeshAgent>();
        EntityData.Instance.ActiveMapEntityList.Add(spawnedEnemy);
    }

    private void SetEnemyPath(GameObject spawnedEnemy, NavMeshAgent enemyAgent)
    {
        spawnedEnemy.transform.position = LevelData.Instance.AgentStartPoint;
        enemyAgent.enabled = true;
        enemyAgent.SetDestination(LevelData.Instance.AgentEndPoint);
    }
}