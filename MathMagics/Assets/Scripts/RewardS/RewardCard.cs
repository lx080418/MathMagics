using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCard : MonoBehaviour
{
    public TMP_Text descriptionText;
    public Image itemImage;
    public Image background;
    public Sprite[] weaponSprites;
    public Sprite potionSprite;
    public Color[] rarityColors;

    public void Initialize(RewardOption option)
    {
        descriptionText.text = option.description;

        switch (option.rarity)
        {
            case Rarity.Common:
                background.color = rarityColors[0];
                break;
            case Rarity.Rare:
                background.color = rarityColors[1];
                break;
            case Rarity.Epic:
                background.color = rarityColors[2];
                break;
        }

        switch (option.weaponName)
        {
            case "Subtract":
                itemImage.sprite = weaponSprites[0];
                break;
            case "Add":
                itemImage.sprite = weaponSprites[1];
                break;
            case "Multiply":
                itemImage.sprite = weaponSprites[2];
                break;
            case "Divide":
                itemImage.sprite = weaponSprites[3];
                break;
            case "Potion":
                itemImage.sprite = potionSprite;
                break;
            default:
                itemImage.sprite = null;
                break;
        }

    }
}
