using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public GameObject rewardUI;
    public TMP_Text[] rewardTexts;
    private List<RewardOption> rewardPool = new();

    private Dictionary<string, float> weaponDropChances = new Dictionary<string, float>()
    {
        {"Subtract", 0.3f},
        {"Add", 0.3f},
        {"Multiply", 0.2f},
        {"Divide", 0.2f},
    };
    void Start()
    {
        rewardUI.SetActive(false);
    }

    public void ShowRewardOptions()
    {
        Time.timeScale = 0f;
        rewardUI.SetActive(true);

        GenerateRewardPool();

        for (int i = 0; i < rewardTexts.Length; i++)
        {
            RewardOption option = rewardPool[i];
            rewardTexts[i].text = $"{option.rarity} - {option.description} (+{option.levelIncrease})";
        }
    }

    private void GenerateRewardPool()
    {
        rewardPool.Clear();

        for (int i = 0; i < 3; i++)
        {
            string chosenWeapon = GetRandomWeaponByDropChance();

            Rarity rarity = GetRandomRarity();
            int level = rarity switch
            {
                Rarity.Common => 1,
                Rarity.Rare => Random.Range(1, 3),
                Rarity.Epic => 3,
                _ => 1
            };

            RewardOption option = new RewardOption(chosenWeapon, rarity, level);
            rewardPool.Add(option);
        }
    }

    private Rarity GetRandomRarity()
    {
        float roll = Random.value;
        if (roll < 0.6f) return Rarity.Common;
        if (roll < 0.75) return Rarity.Rare;
        return Rarity.Epic;
    }

    public void SelectReward(int index)
    {
        RewardOption chosen = rewardPool[index];

        Debug.Log($"[RewardSystem] Chose: {chosen.rarity} → +{chosen.levelIncrease} to w1");

        // Apply reward to w1
        WeaponHandler.Instance.GetWeaponByName(chosen.weaponName)?.IncreaseLevelBy(chosen.levelIncrease);

        rewardUI.SetActive(false);
        Time.timeScale = 1f; // resume game
    }

    private string GetRandomWeaponByDropChance()
    {
        float total = 0f;
        foreach (float weight in weaponDropChances.Values)
            total += weight;

        float roll = Random.value * total;
        float cumulative = 0f;

        foreach (var pair in weaponDropChances)
        {
            cumulative += pair.Value;
            if (roll <= cumulative)
                return pair.Key;
        }

        // Fallback
        return weaponDropChances.Keys.First();
    }

    public void SetWeaponDropChance(string weaponName, float newChance)
    {
        if (weaponDropChances.ContainsKey(weaponName))
        {
            weaponDropChances[weaponName] = newChance;
            Debug.Log($"[RewardSystem] Drop chance for {weaponName} set to {newChance}");
        }
        else
        {
            Debug.LogWarning($"[RewardSystem] Weapon '{weaponName}' not found in drop chance dictionary.");
        }
    }


}
