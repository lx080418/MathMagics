using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PotionUI : MonoBehaviour
{
    [SerializeField] private PlayerPotion playerPotion;
    public TMP_Text potionAmountText;
    public Image potionImage;

    private void Awake()
    {
        playerPotion.OnPotionChanged += HandlePotionChanged;
    }

    private void HandlePotionChanged(Fraction potionAmount, bool hasPotion)
    {
        if (hasPotion)
        {
            potionImage.color = new Color(1, 1, 1, 1);
            potionAmountText.gameObject.SetActive(true);
            potionAmountText.text = potionAmount.ToString();
        }
        else
        {
            potionAmountText.gameObject.SetActive(false);
            potionImage.color = new Color(1, 1, 1, 0);
        }
    }
}
