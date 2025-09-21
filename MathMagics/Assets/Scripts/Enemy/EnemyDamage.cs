using System;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public string damageExpression = "-1"; // Can be "1/2", "2 * 0.5", etc.

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
        Fraction health = playerHealth.GetCurrentHealth(); //returns some Fraction 3 / 5
                                                           //if(health.Numerator < 0)



        //UnityEngine.Random.Range(minInclusive, maxExclusive);
        // return string damage expression
        //
        return "-1";
    }

}
