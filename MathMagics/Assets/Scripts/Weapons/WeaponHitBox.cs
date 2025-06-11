using UnityEngine;

public class WeaponHitbox2D : MonoBehaviour
{
    [HideInInspector] public string damageExpression;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log($"[Hitbox2D] Hit enemy with: {damageExpression}");
                enemyHealth.ApplyDamageExpression(damageExpression);
            }
        }
    }
}
