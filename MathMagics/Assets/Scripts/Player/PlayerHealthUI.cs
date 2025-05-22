using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI hpText;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        UpdateUI(); // show initial HP
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerHealth == null || hpText == null) return;

        string current = playerHealth.GetCurrentHealth().ToString();
        string max = playerHealth.GetMaxHealth().ToString();

        hpText.text = $"HP: {current} / {max}";
    }
}
