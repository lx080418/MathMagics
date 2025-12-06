using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public string damageExpression = "-1"; // Can be "-1/2", "*2", etc.

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enemy hit something.");
        if (other.CompareTag("PlayerHitbox"))
        {
            Debug.Log("Enemy hit Player.");
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log($"[EnemyDamage] Applying damage: {damageExpression}");
                damageExpression = GenerateDamageExpression(playerHealth);
                playerHealth.UpdatePlayerHP(damageExpression);
            }
        }
    }

    private string GenerateDamageExpression(PlayerHealth playerHealth)
    {
        Fraction health = playerHealth.GetCurrentHealth();
        if (SettingsManager.Instance.easyMode)
        {
            if (health.Numerator > 0)
            {
                return "+0";
            }
            return "+0";
        }
        //UnityEngine.Random.Range(minInclusive, maxExclusive);
        // return string damage expression
        //
        int num = UnityEngine.Random.Range(1, GameManager.instance.stageLevel * 2 + (int)health.Numerator / 50);
        if (health.Denominator != 1)
        {
            return "*" + num;
        }
        if (health.Numerator < 0)
        {
            return "+" + num;
        }
        return "-" + num;
    }

}
