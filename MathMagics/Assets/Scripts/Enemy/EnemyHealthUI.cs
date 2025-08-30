using UnityEngine;
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private TextMeshProUGUI hpText;    

    private void Update()
    {
        if (enemyHealth == null || hpText == null) return;

        hpText.text = enemyHealth.GetCurrentHealth().ToString();
    }

}
