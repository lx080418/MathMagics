using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] private string startingHealth = "3";
    private Fraction currentHealth;
    public event Action OnEnemyDied;
    private void Awake()
    {
        //GameManager.instance.stageLevel 
        
        currentHealth = Evaluate(startingHealth);
    }

    private void Start()
    {
        if (CompareTag("Boss"))
        {
            int level = GameManager.instance != null ? GameManager.instance.stageLevel : 1;

            startingHealth = level switch
            {
                1 => "66",
                2 => "-77",
                3 => "101/105",
                4 => "999999",
                _ => "100"
            };

            Debug.Log($"[EnemyHealth] Boss detected. Setting HP for level {level}: {startingHealth}");
        }
       
    }

    public void ApplyDamageExpression(string expression)
    {
        Debug.Log($"[EnemyHealth] ApplyDamageExpression called on {gameObject.name} with expression: {expression}");
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
        //Destroy(gameObject);
        OnEnemyDied?.Invoke();
        RewardSystem rewardSystem = FindObjectOfType<RewardSystem>();
        if (rewardSystem != null)
        {
            if (CompareTag("Boss"))
            {
                rewardSystem.ShowBossRewardOptions();
            }
            else
            {
                rewardSystem.ShowRewardOptions();
            }
        }

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
