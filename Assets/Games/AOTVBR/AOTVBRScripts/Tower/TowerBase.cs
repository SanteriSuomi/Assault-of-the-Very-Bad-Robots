using System.Collections;
using System.Collections.Generic;
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
        private int nearEnemiesArrayLength = 5;
        private Collider[] nearEnemies;

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
        private float damageTimerInterval = 1;
        [SerializeField]
        private float checkRadius = 3.25f;
        private float damageTimer;

        protected bool isPlacing;

        public void IsCurrentPlacing(bool value)
            => isPlacing = value;

        private void Start()
        {
            nearEnemies = new Collider[nearEnemiesArrayLength];
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
            (bool, EnemyBase) value = GetEnemies();
            if (value.Item1)
            {
                EnemyDetectedEvent(value.Item2.transform.position);
                DamageEnemyOnDetection(value.Item2);
                PlayAttackAudio();
            }
            else
            {
                ResetTower();
            }
        }

        private (bool, EnemyBase) GetEnemies()
        {
            int nearEnemiesAmount = Physics.OverlapSphereNonAlloc(transform.position, checkRadius, 
                nearEnemies, enemyLayers);
            if (nearEnemiesAmount == 1)
            {
                return (true, nearEnemies[0].GetComponent<EnemyBase>());
            }
            else if (nearEnemiesAmount > 1)
            {
                EnemyBase lowestEnemy = GetEnemyBases().Min();
                return (true, lowestEnemy);
            }

            return (false, null);
        }

        private List<EnemyBase> GetEnemyBases()
        {
            List<EnemyBase> activeEnemyList = new List<EnemyBase>(nearEnemiesArrayLength);
            for (int i = 0; i < nearEnemies.Length; i++)
            {
                if (nearEnemies[i] != null
                    && nearEnemies[i].TryGetComponent(out EnemyBase enemy))
                {
                    activeEnemyList.Add(enemy);
                }
            }

            return activeEnemyList;
        }

        protected abstract void EnemyDetectedEvent(Vector3 enemyPosition);

        protected virtual void DamageEnemyOnDetection(EnemyBase enemy)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageTimerInterval)
            {
                damageTimer = 0;
                enemy.TakeDamage(Damage);
            }
        }

        protected abstract void PlayAttackAudio();

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