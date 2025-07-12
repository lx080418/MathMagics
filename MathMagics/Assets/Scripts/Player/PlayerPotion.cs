using System;
using UnityEngine;

public class PlayerPotion : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    private Fraction potionHealth = new Fraction(0);
    private bool hasPotion = false;

    public delegate void PotionChanged(Fraction potionAmount, bool hasPotion);
    public event PotionChanged OnPotionChanged;

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
        SetPotion("10");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && hasPotion)
        {
            UsePotion();
        }
    }

    public void ModifyPotionAmount(string expression)
    {
        ExpressionTree tree = new ExpressionTree();
        tree.BuildFromInfix(potionHealth.ToString() + expression);
        potionHealth = tree.Evaluate();

        hasPotion = potionHealth.Numerator > 0;
        OnPotionChanged?.Invoke(potionHealth, hasPotion);
    }

    public void SetPotion(string value)
    {
        potionHealth = playerHealth.Evaluate(value);
        Debug.Log($"Setting potion to {value}, numerator = {potionHealth.Numerator}");

        hasPotion = potionHealth.Numerator != 0;
        OnPotionChanged?.Invoke(potionHealth, hasPotion);
    }

    public void UsePotion()
    {
        playerHealth.UpdatePlayerHP("+" + potionHealth.ToString());
        potionHealth = new Fraction(0);
        hasPotion = false;
        OnPotionChanged?.Invoke(potionHealth, false);
    }

    public Fraction GetPotionHealth() => potionHealth;
    public bool HasPotion() => hasPotion;
}
