using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace AOTVBR
{
    public class GameManager : Singleton<GameManager>
    {
        public delegate void GameMenuHide();
        public event GameMenuHide GameMenuHideEvent;

        [SerializeField]
        private TextMeshProUGUI countdownText = default;
        [SerializeField]
        private TextMeshProUGUI healthText = default;
        [SerializeField]
        private TextMeshProUGUI fundsText = default;
        [SerializeField]
        private TextMeshProUGUI timeText = default;
        [SerializeField]
        private EnemyBasic enemyBasic = default;
        [SerializeField]
        private EnemyStrong enemyStrong = default;
        [SerializeField]
        private GameObject gameOverScreen = default;
        private WaitForSeconds countdownLoopWFS;
        private WaitForSeconds resetGameWFS;
        private WaitForSeconds decreaseSpawnWFS;

        [SerializeField]
        private int gameStartCountdownTime = 3;

        [SerializeField]
        private float enemyBasicSpawnInterval = 5;
        [SerializeField]
        private float enemyStrongSpawnInterval = 25;
        [SerializeField]
        private float spawnIntervalDecreaseTime = 15;
        [SerializeField]
        private float spawnIntervalDecrease = 0.975f;
        [SerializeField]
        private float minSpawnInterval = 0.2f;
        [SerializeField]
        private float countdownLoopInterval = 1;
        [SerializeField]
        private float resetGameDelay = 2.5f;
        private float enemyBasicSpawnIntervalDefault;
        private float enemyStrongSpawnIntervalDefault;
        private float enemyTimerBasic;
        private float enemyTimerStrong;
        private float textTime;

        private bool hasGameResetted;
        private bool hasDecreasedSpawnInterval;
        private bool finishedGame;

        protected override void Awake()
        {
            countdownLoopWFS = new WaitForSeconds(countdownLoopInterval);
            resetGameWFS = new WaitForSeconds(resetGameDelay);
            decreaseSpawnWFS = new WaitForSeconds(spawnIntervalDecreaseTime);

            GameState.Instance.GameStartedEvent += GameStart;
            // Store the default spawn intervals to reset them later.
            enemyBasicSpawnIntervalDefault = enemyBasicSpawnInterval;
            enemyStrongSpawnIntervalDefault = enemyStrongSpawnInterval;
        }

        private void GameReset()
        {
            // Reset most of the game variables back to zero.
            StopAllCoroutines();
            hasDecreasedSpawnInterval = false;
            finishedGame = false;
            enemyTimerBasic = 0;
            enemyTimerStrong = 0;
            textTime = 0;
            enemyBasicSpawnInterval = enemyBasicSpawnIntervalDefault;
            enemyStrongSpawnInterval = enemyStrongSpawnIntervalDefault;
            PlayerData.Instance.Health = PlayerData.Instance.StartingHealth;
        }

        private void GameStart()
        {
            if (!hasGameResetted)
            {
                hasGameResetted = true;
                GameReset();
            }

            StartCoroutine(UpdateLoop());
        }

        private IEnumerator UpdateLoop()
        {
            yield return StartCoroutine(Countdown());
            while (enabled)
            {
                UpdateGameState();
                yield return null;
            }
        }

        private IEnumerator Countdown()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            for (int i = gameStartCountdownTime; i >= 0; i--)
            {
                countdownText.text = i.ToString();
                yield return countdownLoopWFS;
            }

            countdownText.text = string.Empty;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void UpdateGameState()
        {
            if (PlayerData.Instance.Health <= 0 
                && !finishedGame)
            {
                finishedGame = true;
                StartCoroutine(DeactivateGame());
            }

            UpdateGameUI();
            SpawnEnemyBasic(enemyBasic, enemyBasicSpawnInterval);
            SpawnEnemyStrong(enemyStrong, enemyStrongSpawnInterval);

            if (!hasDecreasedSpawnInterval
                && enemyBasicSpawnInterval >= minSpawnInterval)
            {
                hasDecreasedSpawnInterval = true;
                StartCoroutine(DecreaseSpawnInterval());
            }
        }

        private IEnumerator DeactivateGame()
        {
            gameOverScreen.SetActive(true);
            yield return resetGameWFS;
            gameOverScreen.SetActive(false);

            StopAllCoroutines();
            hasGameResetted = false;
            GameMenuHideEvent.Invoke();
            GameState.Instance.SetState((int)GameStates.GenerateMap);
        }

        private void UpdateGameUI()
        {
            healthText.text = $"{PlayerData.Instance.Health}";
            fundsText.text = $"{Math.Round(PlayerData.Instance.Funds, 2)}";
            textTime += Time.deltaTime;
            timeText.text = $"{Mathf.RoundToInt(textTime)}";
        }

        private void SpawnEnemyBasic(EnemyBase enemy, float interval)
        {
            enemyTimerBasic += Time.deltaTime;
            if (enemyTimerBasic >= interval)
            {
                enemyTimerBasic = 0;
                InitializeEnemy(enemy, out EnemyBase spawnedEnemy, out NavMeshAgent enemyAgent);
                SetEnemyPath(spawnedEnemy, enemyAgent);
            }
        }

        private void SpawnEnemyStrong(EnemyBase enemy, float interval)
        {
            enemyTimerStrong += Time.deltaTime;
            if (enemyTimerStrong >= interval)
            {
                enemyTimerStrong = 0;
                InitializeEnemy(enemy, out EnemyBase spawnedEnemy, out NavMeshAgent enemyAgent);
                SetEnemyPath(spawnedEnemy, enemyAgent);
            }
        }

        private void InitializeEnemy(EnemyBase enemy, out EnemyBase spawnedEnemy, out NavMeshAgent enemyAgent)
        {
            if (enemy is EnemyBasic)
            {
                spawnedEnemy = EnemyBasicPool.Instance.Get();
            }
            else
            {
                spawnedEnemy = EnemyStrongPool.Instance.Get();
            }

            enemyAgent = spawnedEnemy.GetComponent<NavMeshAgent>();
            EntityData.Instance.ActiveMapEntities.Add(spawnedEnemy.gameObject);
        }

        private void SetEnemyPath(EnemyBase spawnedEnemy, NavMeshAgent enemyAgent)
        {
            spawnedEnemy.transform.position = LevelData.Instance.AgentStartPoint;
            // Enable navmeshagent after placing it on the navmesh.
            enemyAgent.enabled = true;
            enemyAgent.SetDestination(LevelData.Instance.AgentEndPoint);
        }

        private IEnumerator DecreaseSpawnInterval()
        {
            yield return decreaseSpawnWFS;
            enemyBasicSpawnInterval -= spawnIntervalDecrease;
            enemyStrongSpawnInterval -= spawnIntervalDecrease;
            hasDecreasedSpawnInterval = false;
        }
    }
}