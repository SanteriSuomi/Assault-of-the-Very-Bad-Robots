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
        IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
        if (enemy != null)
        {
            PlayManager.Instance.Health -= enemy.Damage;
            StartCoroutine(DamageText(enemy, collision));
        }
    }

    private IEnumerator DamageText(IEnemy enemy, Collider collision)
    {
        damageText.text = $"{enemy.Name} has dealt {enemy.Damage} to your base!";
        Destroy(collision.gameObject);
        yield return new WaitForSeconds(2);
        damageText.text = string.Empty;
    }
}