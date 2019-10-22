using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; set; }

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
    private int startingHealth = 100;
    [SerializeField]
    private int startingFunds = 25;

    public int Health { get; set; }
    public int Funds { get; set; }

    private float enemyTimer;

    private bool hasGameStarted = false;
    private bool setHealth = false;
    private bool hasCountdownPlayed = false;
    private bool continueGame = false;

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
            Health = startingHealth;
            Funds = startingFunds;
        }
    }

    private void GameEnded()
    {
        hasGameStarted = false;
        GameState.Instance.SetState(2);
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
                if (Health < 0)
                {
                    GameEnded();
                }

                healthText.text = $"Health: {Health}";
                fundsText.text = $"Funds: {Funds}";

                SpawnEnemies(enemyBasic, 4.5f);
            }
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
}
