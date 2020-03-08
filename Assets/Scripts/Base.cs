﻿using System.Collections;
using TMPro;
using UnityEngine;

namespace AOTVBR
{
    public class Base : MonoBehaviour
    {
        private TextMeshProUGUI damageText;
        private WaitForSeconds showDamageTextWFS;
        [SerializeField]
        private float showDamageTextDelay = 2.5f;
        private bool isShowingDamageText;

        private void Awake()
        {
            damageText = GameObject.Find("BaseDamageText").GetComponent<TextMeshProUGUI>();
            showDamageTextWFS = new WaitForSeconds(showDamageTextDelay);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.TryGetComponent(out EnemyBase enemy))
            {
                // Subtract health from the player.
                PlayerData.Instance.Health -= enemy.Damage;
                if (!isShowingDamageText)
                {
                    isShowingDamageText = true;
                    StartCoroutine(ShowDamageText(enemy));
                }
            }
        }

        private IEnumerator ShowDamageText(EnemyBase enemy)
        {
            damageText.text = $"{enemy.Name} has dealt {enemy.Damage} to your base!";
            enemy.Die();
            yield return showDamageTextWFS;
            damageText.text = string.Empty;
            isShowingDamageText = false;
        }
    }
}