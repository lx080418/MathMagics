using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI hpText;

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        // Subscribe to health updates
        playerHealth.OnHealthChanged += UpdateUI;

        // Initialize UI immediately
        UpdateUI(playerHealth.GetCurrentHealth());
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateUI;
    }

    private void UpdateUI(Fraction currentHealth)
    {
        if (hpText != null)
            hpText.text = $"HP: {currentHealth}";
    }
}
