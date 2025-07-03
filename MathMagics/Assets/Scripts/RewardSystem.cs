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

    private Dictionary<int, Dictionary<string, float>> weaponDropChances = new Dictionary<int, Dictionary<string, float>>()
    {
        { 1, new Dictionary<string, float>
            {
                {"Subtract", 1.0f},
                {"Add", 0.0f},
                {"Multiply", 0.0f},
                {"Divide", 0.0f}
            }
        },
                { 2, new Dictionary<string, float>
            {
                {"Subtract", .3f},
                {"Add", 0.7f},
                {"Multiply", 0.0f},
                {"Divide", 0.0f}
            }
        },
                { 3, new Dictionary<string, float>
            {
                {"Subtract", 0.3f},
                {"Add", 0.3f},
                {"Multiply", 0.4f},
                {"Divide", 0.0f}
            }
        },
                { 4, new Dictionary<string, float>
            {
                {"Subtract", 0.2f},
                {"Add", 0.2f},
                {"Multiply", 0.2f},
                {"Divide", 0.4f}
            }
        },


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

        string[] weaponNames = new string[] { "Subtract", "Add", "Multiply", "Divide" };

        for (int i = 0; i < 3; i++)
        {
            if (Random.value < 0.2f) // 25% chance to include a health reward
            {
                Rarity rarity = GetRandomRarity();
                int healthAmount = rarity switch
                {
                    Rarity.Common => 5,
                    Rarity.Rare => 10,
                    Rarity.Epic => 15,
                    _ => 5
                };

                rewardPool.Add(new RewardOption("Gain Potion (+HP)", rarity, healthAmount, RewardType.Health));
            }
            else
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

                rewardPool.Add(new RewardOption(chosenWeapon, rarity, level));
            }
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

        Debug.Log($"[RewardSystem] Selected reward: {chosen.description}");

        if (chosen.rewardType == RewardType.Weapon)
        {
            WeaponHandler.Instance.GetWeaponByName(chosen.weaponName)
                ?.IncreaseLevelBy(chosen.levelIncrease);
        }
        else if (chosen.rewardType == RewardType.Health)
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.UpdatePlayerHP("+" + chosen.levelIncrease.ToString());
            }
        }

        rewardUI.SetActive(false);
        Time.timeScale = 1f;
    }


    private string GetRandomWeaponByDropChance()
    {
        int stage = GameManager.instance != null ? GameManager.instance.stageLevel : 1;

        if (!weaponDropChances.ContainsKey(stage))
        {
            Debug.LogWarning($"[RewardSystem] No drop table for stage {stage}. Using stage 1.");
            stage = 1;
        }

        var dropTable = weaponDropChances[stage];

        float total = dropTable.Values.Sum();
        float roll = Random.value * total;
        float cumulative = 0f;

        foreach (var pair in dropTable)
        {
            cumulative += pair.Value;
            if (roll <= cumulative)
                return pair.Key;
        }

        return dropTable.Keys.First(); // fallback
    }


    public void SetWeaponDropChance(int stage, string weaponName, float newChance)
    {
        if (!weaponDropChances.ContainsKey(stage))
        {
            Debug.LogWarning($"[RewardSystem] No drop table for stage {stage}");
            return;
        }

        var dropTable = weaponDropChances[stage];

        if (dropTable.ContainsKey(weaponName))
        {
            dropTable[weaponName] = newChance;
            Debug.Log($"[RewardSystem] Drop chance for {weaponName} in stage {stage} set to {newChance}");
        }
        else
        {
            Debug.LogWarning($"[RewardSystem] Weapon '{weaponName}' not found in stage {stage} drop table.");
        }
    }



}
