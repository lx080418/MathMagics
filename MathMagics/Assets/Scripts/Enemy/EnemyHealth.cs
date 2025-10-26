using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] private string startingHealth = "3";
    private Fraction currentHealth;
    public event Action OnEnemyDied;

    private int level;
    private int numOfEnemy;
    private void Awake()
    {
        level = GameManager.instance.stageLevel;
        numOfEnemy = GameManager.instance.numOfEnemy;

        if (level == 1)
        {
            startingHealth = ((int)UnityEngine.Random.Range(1 + GameManager.instance.numOfEnemy / 1.5f, GameManager.instance.numOfEnemy * 1.5f + 4)).ToString();
        }
        else if (level == 2)
        {
            int n = UnityEngine.Random.Range(1, 100);

            if (n < 25) // 25% chance to have positive health
            {
                startingHealth = ((int)UnityEngine.Random.Range(4 + GameManager.instance.numOfEnemy / 1.5f, GameManager.instance.numOfEnemy * 1.6f + 7)).ToString();
            }
            else  // 75 % chance to have negative health
            {
                startingHealth = ((int)UnityEngine.Random.Range(-GameManager.instance.numOfEnemy * 1.5f - 4, -GameManager.instance.numOfEnemy / 1.5f - 2)).ToString();
            }
        }
        else if (level == 3)
        {
            int upper = (int)UnityEngine.Random.Range(4 + GameManager.instance.numOfEnemy * 3, GameManager.instance.numOfEnemy * 4 + 6);
            int lower = (int)UnityEngine.Random.Range(2, GameManager.instance.numOfEnemy / 2 + 2);
            int n = UnityEngine.Random.Range(1, 100);
            if (n < 20)
            {
                startingHealth = ((int)(upper * 0.7)).ToString();
            }
            else if (GameManager.instance.numOfEnemy > 4)
            {
                while (upper % lower == 0 || lower % 7 == 0 || lower % 11 == 0 || lower % 13 == 0 || lower % 17 == 0 || lower % 19 == 0 || lower % 23 == 0 || lower % 29 == 0 || lower % 31 == 0 || lower % 37 == 0 || lower % 41 == 0 || lower % 43 == 0 || lower % 47 == 0)
                {
                    lower++;
                }
                startingHealth = upper.ToString() + "/" + lower.ToString();
            }
            else
            {
                startingHealth = upper.ToString() + "/" + lower.ToString();
            }
            if (n > 10 && n < 50)
            {
                startingHealth = "-" + startingHealth;
            }
        }
        else
        {
            int upper = (int)UnityEngine.Random.Range(6+GameManager.instance.numOfEnemy*7, GameManager.instance.numOfEnemy*10+8);
            int lower = (int)UnityEngine.Random.Range(2+GameManager.instance.numOfEnemy, GameManager.instance.numOfEnemy*3+6);
            int n = UnityEngine.Random.Range(1, 100);
            if(n < 40)
            {
                startingHealth = (upper*3).ToString();
            }
            else
            {
                while(upper % lower ==0 || lower%11==0 || lower%13==0 || lower%17==0 || lower%19==0 || lower%23==0 || lower%29==0 || lower%31==0 || lower%37==0 || lower%41==0 || lower%43==0 || lower%47==0 || lower%53==0 || lower%59==0 || lower%61==0 || lower%67==0 || lower%71==0 || lower%73==0 || lower%79==0)
                {
                    lower++;
                }
                startingHealth = upper.ToString()+"/"+lower.ToString();
            }
            if(n>20 && n<60)
            {
                startingHealth = "-" + startingHealth;
            }
        }
        
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
