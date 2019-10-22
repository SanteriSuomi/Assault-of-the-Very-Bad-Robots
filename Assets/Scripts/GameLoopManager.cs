using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance { get; set; }

    [SerializeField]
    private TextMeshProUGUI gameStartedCountdownText = default;
    [SerializeField]
    private TextMeshProUGUI healthText = default;
    [SerializeField]
    private TextMeshProUGUI fundsText = default;
    [SerializeField]
    private GameObject enemyBasic = default;

    [SerializeField]
    private int gameStartCountdownTime = 3;
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private int funds = 10;
    [SerializeField]
    private float enemySpawnTime = 4.5f;

    public int Health { get; set; }
    public int Funds { get; set; }

    private float enemyTimer;

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

        GameState.Instance.GameStartedEvent += GameStarted;
    }

    private void GameStarted()
    {
        hasGameStarted = true;

        if (!setHealth)
        {
            setHealth = true;
            Health = health;
            Funds = funds;
        }
    }

    private void GameEnded()
    {
        GameReset();
        GameState.Instance.SetState(2);
    }

    private void GameReset()
    {
        hasGameStarted = false;
        setHealth = false;
        hasCountdownPlayed = false;
        continueGame = false;
    }

    private void Update()
    {
        if (hasGameStarted)
        {
            Countdown();
            GameLoop();
        }
    }

    private void Countdown()
    {
        if (!hasCountdownPlayed)
        {
            hasCountdownPlayed = true;
            StartCoroutine(PlayMapCountdown());
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
        if (continueGame)
        {
            if (Health < 0)
            {
                GameEnded();
            }

            healthText.text = $"Health: {Health}";
            fundsText.text = $"Funds: {Funds}";

            SpawnEnemies(enemyBasic, enemySpawnTime);
        }
    }

    private void SpawnEnemies(GameObject enemy, float time)
    {
        enemyTimer += Time.deltaTime;
        if (enemyTimer >= time)
        {
            enemyTimer = 0;
            GameObject spawnedEnemy = Instantiate(enemy);
            NavMeshAgent enemyAgent = spawnedEnemy.GetComponent<NavMeshAgent>();
            spawnedEnemy.transform.position = LevelData.Instance.AgentStartPoint + new Vector3(0, 0.4f, 0);
            enemyAgent.enabled = true;
            enemyAgent.SetDestination(LevelData.Instance.AgentEndPoint);
        }
    }
}