using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagicStoneUpgradeManager : MonoBehaviour
{
    public PlayerMagicStone playerMagicStone;
    [Header("Weapon Level UIs")]
    public List<WeaponLevelUI> weaponLevelUIs = new List<WeaponLevelUI>();
    private Dictionary<int, int> weaponUpgradeCosts = new Dictionary<int, int>();

    [Header("UI References")]
    public GameObject upgradeWeaponParent;
    public TMP_Text headerText;
    public Image weaponImage;
    public TMP_Text costText;
    public Sprite[] weaponSprites;

    //Internal
    private int currentWeaponIndex;

    private void Awake()
    {
        SubscribeToWeaponUIs();
        SeedWeaponCostDictionary();
    }

    private void SubscribeToWeaponUIs()
    {
        foreach (var weaponLevelUI in weaponLevelUIs)
        {
            weaponLevelUI.OnWeaponSlotClicked += HandleWeaponSlotClicked;
        }
    }

    private void HandleWeaponSlotClicked(int index)
    {
        Weapon w = WeaponHandler.Instance.GetWeapons()[index];
        if(w.getIsLocked())
        {
            return;
        }
        currentWeaponIndex = index;
        //initialize the UI with the proper information
        
        headerText.text = $"Would you like to upgrade the {w.getName()} wand?";
        weaponImage.sprite = weaponSprites[index];
        costText.text = weaponUpgradeCosts[index].ToString();

        if (playerMagicStone.GetMagicStones() >= weaponUpgradeCosts[currentWeaponIndex])
        {
            costText.color = new Color(0.12f, 0.62f, 0.28f);
        }
        else
        {
            costText.color = new Color(0.66f, 0.17f, 0.19f);
        }
        upgradeWeaponParent.SetActive(true);
    }



    public void UpgradeWeapon()
    {
        // Check if the player has enough magic stones to upgrade the weapon
        if (playerMagicStone.GetMagicStones() >= weaponUpgradeCosts[currentWeaponIndex])
        {
            // Deduct the cost from the player's magic stones
            playerMagicStone.LoseMagicStone(weaponUpgradeCosts[currentWeaponIndex]);

            // Upgrade the weapon level
            Weapon w = WeaponHandler.Instance.GetWeapons()[currentWeaponIndex];
            w.UnlockNextLevel();

            weaponUpgradeCosts[currentWeaponIndex]++;

            HandleWeaponSlotClicked(currentWeaponIndex); // Refresh the UI with updated information
        }
        else
        {
            // Handle the case where the player doesn't have enough magic stones
            Debug.Log("Not enough magic stones to upgrade the weapon.");
        }
    }

    private void SeedWeaponCostDictionary()
    {
        for (int i = 0; i < weaponLevelUIs.Count; i++)
        {
            weaponUpgradeCosts.Add(i, 1); // Initialize the cost for each weapon to 1
        }
    }
}
