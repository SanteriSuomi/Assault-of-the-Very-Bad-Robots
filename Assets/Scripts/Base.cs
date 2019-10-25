using System.Collections;
using UnityEngine;
using TMPro;

public class Base : MonoBehaviour
{
    private TextMeshProUGUI damageText;

    private void Awake()
    {
        damageText = GameObject.Find("BaseDamageText").GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Retrieve the enemy interface component.
        IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
        // Make sure it's an enemy by checking for null.
        if (enemy != null)
        {
            // Subtract health from the player.
            PlayManager.Instance.Health -= enemy.Damage;
            // Show a damage text that shows up for a small time.
            StartCoroutine(DamageText(enemy));
        }
    }

    private IEnumerator DamageText(IEnemy enemy)
    {
        damageText.text = $"{enemy.Name} has dealt {enemy.Damage} to your base!";
        enemy.Die();
        yield return new WaitForSeconds(2.5f);
        // Reset the text by making it empty.
        damageText.text = string.Empty;
    }
}