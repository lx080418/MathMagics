using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private string maxHealthExpression = "3";
    private Fraction maxHealth;
    private Fraction currentHealth;

    void Start()
    {
        maxHealth = EvaluateExpression(maxHealthExpression);
        currentHealth = maxHealth;

        Debug.Log($"[PlayerHealth] Initialized: {currentHealth} HP");
    }

    public void TakeDamage(string damageExpression)
    {
        if (IsDead())
        {
            Debug.Log("[PlayerHealth] Already dead, ignoring damage.");
            return;
        }

        Fraction damage = EvaluateExpression(damageExpression);
        currentHealth -= damage;

        if (currentHealth.Numerator < 0)
            currentHealth = new Fraction(0);

        Debug.Log($"[PlayerHealth] Took {damage} damage. Remaining HP: {currentHealth}");

        if (IsDead())
            Die();
    }

    public bool IsDead()
    {
        return currentHealth.Numerator <= 0;
    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] Player has died!");
        // Add game-over, animation, disable movement, etc.
    }

    private Fraction EvaluateExpression(string expr)
    {
        try
        {
            ExpressionTree tree = new ExpressionTree();
            tree.BuildFromInfix(expr);
            return tree.Evaluate();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PlayerHealth] Error evaluating expression '{expr}': {e.Message}");
            return new Fraction(0);
        }
    }

    public Fraction GetCurrentHealth() => currentHealth;
    public Fraction GetMaxHealth() => maxHealth;
}
