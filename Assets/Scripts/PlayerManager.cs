using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace AOTVBR
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public int Health { get; set; }
        public float Funds { get; set; }

        public delegate void GameMenuHide();
        public event GameMenuHide GameMenuHideEvent;

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
        private GameObject gameOverScreen = default;
        private WaitForSeconds countdownLoopWFS;

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
        [SerializeField]
        private float spawnIntervalDecreaseTime = 15;
        [SerializeField]
        private float spawnIntervalDecrease = 0.975f;
        [SerializeField]
        private float minSpawnInterval = 0.2f;
        [SerializeField]
        private float countdownLoopInterval = 1;
        private float enemyBasicSpawnIntervalDefault;
        private float enemyStrongSpawnIntervalDefault;
        private float enemyTimerBasic;
        private float enemyTimerStrong;
        private float textTime;

        #region Game Flow
        private bool hasGameResetted;
        private bool decreasedInterval;
        #endregion

        protected override void Awake()
        {
            countdownLoopWFS = new WaitForSeconds(countdownLoopInterval);
            GameState.Instance.GameStartedEvent += GameStart;
            // Store the default spawn intervals to reset them later.
            enemyBasicSpawnIntervalDefault = enemyBasicSpawnInterval;
            enemyStrongSpawnIntervalDefault = enemyStrongSpawnInterval;
        }

        public void GameReset()
        {
            // Reset most of the game variables back to zero.
            StopCoroutine(nameof(UpdateLoop));
            StopCoroutine(nameof(CountdownLoop));
            decreasedInterval = false;
            enemyTimerBasic = 0;
            enemyTimerStrong = 0;
            textTime = 0;
            enemyBasicSpawnInterval = enemyBasicSpawnIntervalDefault;
            enemyStrongSpawnInterval = enemyStrongSpawnIntervalDefault;
            Funds = funds;
            Health = health;
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
            yield return StartCoroutine(CountdownLoop());
            Debug.Log("asd");
            while (enabled)
            {
                GameLoop();
            }

            yield return null;
        }

        private IEnumerator CountdownLoop()
        {
            CursorLock(false, CursorLockMode.Locked);
            for (int i = gameStartCountdownTime; i >= 0; i--)
            {
                gameStartedCountdownText.text = i.ToString();
                yield return countdownLoopWFS;
            }

            gameStartedCountdownText.text = string.Empty;
            CursorLock(true, CursorLockMode.None);
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
                // Start a delay and show an ending screen before changing state.
                StartCoroutine(ResetDelay());
            }
            // Update the health, fund and time info on top of the screen.
            UpdateHealthFundTimeText();
            // Spawn the enemies on intervals.
            SpawnEnemyBasic(enemy: enemyBasic, interval: enemyBasicSpawnInterval);
            SpawnEnemyStrong(enemy: enemyStrong, interval: enemyStrongSpawnInterval);
            // Decrease spawn interval every X seconds, while capping it.
            if (!decreasedInterval
                && enemyBasicSpawnInterval >= minSpawnInterval)
            {
                decreasedInterval = true;
                // Decrease spawn interval.
                StartCoroutine(DecreaseSpawnInterval());
            }
        }

        private IEnumerator ResetDelay()
        {
            // Show the game over screen.
            gameOverScreen.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            gameOverScreen.SetActive(false);
            // When dead, make sure reset is false, so game can be resetted again.
            hasGameResetted = false;
            // Make sure to start the game only from the play map button.
            StopCoroutine(nameof(UpdateLoop));
            // Invoke an event to hide the game menus.
            GameMenuHideEvent.Invoke();
            // Set the state to the generate menu.
            GameState.Instance.SetState(2);
        }

        private void UpdateHealthFundTimeText()
        {
            // Updating the necessary data for gameplay.
            healthText.text = $"{Health}";
            fundsText.text = $"{Math.Round(Funds, 2)}";
            // Time text timer.
            textTime += Time.deltaTime;
            timeText.text = $"{Mathf.RoundToInt(textTime)}";
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

        private IEnumerator DecreaseSpawnInterval()
        {
            // Wait 15 seconds every time before decreasing interval.
            yield return new WaitForSeconds(spawnIntervalDecreaseTime);
            // Decrease both interval with percentage.
            enemyBasicSpawnInterval -= spawnIntervalDecrease;
            enemyStrongSpawnInterval -= spawnIntervalDecrease;
            decreasedInterval = false;
        }
    }
}