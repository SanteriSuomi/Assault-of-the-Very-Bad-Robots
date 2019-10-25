using System.Collections;
using UnityEngine;
using TMPro;

public class Base : MonoBehaviour
{
    private TextMeshProUGUI damageText;

    private void Start()
    {
        damageText = GameObject.Find("BaseDamageText").GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        // On collisions with the base, get the enemy component.
        IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
        // Make sure it's an enemy.
        if (enemy != null)
        {
            // Subtract health from the player.
            PlayManager.Instance.Health -= enemy.Damage;
            // Show a damage text that shows up for a small time.
            StartCoroutine(DamageText(enemy, collision));
        }
    }

    private IEnumerator DamageText(IEnemy enemy, Collider collision)
    {
        // Damage text.
        damageText.text = $"{enemy.Name} has dealt {enemy.Damage} to your base!";
        enemy.Die();
        yield return new WaitForSeconds(2);
        // Reset the text by making it empty.
        damageText.text = string.Empty;
    }
}