using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] private string startingHealth = "3";
    private Fraction currentHealth;

    private void Start()
    {
        currentHealth = Evaluate(startingHealth);
    }

    public void ApplyDamageExpression(string expression)
    {
        ExpressionTree tree = new ExpressionTree();
        tree.BuildFromInfix(currentHealth.ToString() + expression);
        currentHealth = tree.Evaluate();

        Debug.Log($"[EnemyHealth] New HP: {currentHealth}");

        if (currentHealth.Numerator == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[EnemyHealth] {gameObject.name} has died.");
        Destroy(gameObject);
    }

    private Fraction Evaluate(string expr)
    {
        try
        {
            ExpressionTree tree = new ExpressionTree();
            tree.BuildFromInfix(expr);
            return tree.Evaluate();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[EnemyHealth] Invalid expression '{expr}': {e.Message}");
            return new Fraction(0);
        }
    }

    public Fraction GetCurrentHealth() => currentHealth;
}
