using System;
using UnityEngine;

public class WeaponHitbox2D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null && WeaponHandler.Instance != null)
            {
                String damageExpression = WeaponHandler.Instance.GetCurrentWeapon().GetDamageExpression();
                Debug.Log($"[Hitbox2D] Hit enemy with: {damageExpression}");
                enemyHealth.ApplyDamageExpression(damageExpression);
            }
        }
    }
}
    