using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    [Header("Player Health")]
    [SerializeField] private string startingHealth = "20";
    private Fraction currentHealth;

    [Header("References")]
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject playerParentObject;

    [Header("Audio")]
    [SerializeField] private AudioClip playerHurtSFX;

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
        Fraction previousHealth = currentHealth;
        currentHealth = tree.Evaluate();

        if(currentHealth < previousHealth)
        {
            AudioManager.Instance.PlayOneShotVariedPitch(playerHurtSFX, 1f, AudioManager.Instance.sfxAMG, .03f);
        }

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

    private void HandleDeath()
    {
        Debug.Log("[PlayerHealth] Player has died!");
        if (loseScreen != null)
            loseScreen.SetActive(true);
        
        Destroy(playerParentObject);
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
