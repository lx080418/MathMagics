using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private string damageExpression = "1"; // Can be "1/2", "2 * 0.5", etc.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log($"[EnemyDamage] Applying damage: {damageExpression}");
                playerHealth.TakeDamage(damageExpression);
            }
        }
    }
}
