using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    [Header("Player Health")]
    [SerializeField] private string startingHealth = "20";
    private Fraction currentHealth;

    [Header("References")]
    [SerializeField] private GameObject loseScreen;

    // Events
    public delegate void HealthChanged(Fraction newHealth);
    public event HealthChanged OnHealthChanged;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHealth = Evaluate(startingHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void UpdatePlayerHP(string expression)
    {
        ExpressionTree tree = new ExpressionTree();
        tree.BuildFromInfix(currentHealth.ToString() + expression);
        currentHealth = tree.Evaluate();

        Debug.Log($"[PlayerHealth] HP changed to: {currentHealth}");

        if (currentHealth.Numerator == 0)
        {
            currentHealth = new Fraction(0);
            OnHealthChanged?.Invoke(currentHealth);
            HandleDeath();
        }
        else
        {
            OnHealthChanged?.Invoke(currentHealth);
        }
    }

    public void TakeDamage(string damageExpression)
    {
        Debug.Log($"[PlayerHealth] Taking damage: {damageExpression}");
        UpdatePlayerHP("-" + damageExpression);
    }

    private void HandleDeath()
    {
        Debug.Log("[PlayerHealth] Player has died!");
        if (loseScreen != null)
            loseScreen.SetActive(true);
        Destroy(gameObject);
    }

    public Fraction GetCurrentHealth() => currentHealth;

    public Fraction Evaluate(string expr)
    {
        try
        {
            ExpressionTree tree = new ExpressionTree();
            tree.BuildFromInfix(expr);
            return tree.Evaluate();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PlayerHealth] Invalid expression '{expr}': {e.Message}");
            return new Fraction(0);
        }
    }
}
