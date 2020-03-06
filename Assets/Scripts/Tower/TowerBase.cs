using System.Collections;
using System.Linq;
using UnityEngine;

namespace AOTVBR
{
    public abstract class TowerBase : MonoBehaviour
    {
        [SerializeField]
        protected Transform turret = default;
        [SerializeField]
        protected LayerMask enemyLayers = default;
        protected AudioSource audioSource;

        [SerializeField]
        private new string name = "Tower";
        public string Name { get => name; }
        [SerializeField]
        private int cost = 10;
        public int Cost { get => cost; }
        [SerializeField]
        private float damage = 10;
        public float Damage { get => damage; }

        [SerializeField]
        private float damageTimerAmount = 1;
        [SerializeField]
        private float checkRadius = 3.25f;
        private float damageTimer;

        protected bool isPlacing;

        public void IsCurrentPlacing(bool enable)
            => isPlacing = enable;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            StartCoroutine(DetectionLoop());
        }

        private IEnumerator DetectionLoop()
        {
            while (enabled)
            {
                if (!isPlacing)
                {
                    DetectEnemies();
                }

                yield return null;
            }
        }

        private void DetectEnemies()
        {
            (bool, EnemyBase) value = GetEnemyInRadius();
            if (value.Item1)
            {
                EnemyDetectedEvent(value.Item2.transform.position);
                DamageWithInterval(value.Item2);
                PlayAudio();
            }
            else
            {
                ResetTower();
            }
        }

        private (bool, EnemyBase) GetEnemyInRadius()
        {
            Collider[] enemyCollisions = Physics.OverlapSphere(transform.position, checkRadius, enemyLayers);
            if (enemyCollisions.Length == 1)
            {
                return (true, enemyCollisions[0].GetComponent<EnemyBase>());
            }
            else if (enemyCollisions.Length > 1)
            {
                EnemyBase[] enemies = GetEnemies(enemyCollisions);
                EnemyBase lowestEnemy = enemies.Min();
                return (true, lowestEnemy);
            }

            return (false, null);
        }

        private EnemyBase[] GetEnemies(Collider[] enemyCollisions)
        {
            EnemyBase[] enemies = new EnemyBase[enemyCollisions.Length];
            for (int i = 0; i < enemyCollisions.Length - 1; i++)
            {
                enemies[i] = enemyCollisions[i].GetComponent<EnemyBase>();
            }

            return enemies;
        }

        protected abstract void EnemyDetectedEvent(Vector3 enemyPosition);

        private void DamageWithInterval(EnemyBase enemy)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageTimerAmount)
            {
                damageTimer = 0;
                enemy.TakeDamage(Damage);
            }
        }

        protected abstract void PlayAudio();

        protected virtual void ResetTower()
        {
            damageTimer = 0;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}