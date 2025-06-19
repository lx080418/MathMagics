using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public GameObject rewardUI;
    public TMP_Text[] rewardTexts;
    private List<RewardOption> rewardPool = new();
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
            Rarity rarity = GetRandomRarity();
            int level = rarity switch
            {
                Rarity.Common => 1,
                Rarity.Rare => Random.Range(1, 3),
                Rarity.Epic => 3,
                _ => 1
            };
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
        WeaponHandler.Instance.GetWeaponByName("Subtract")?.IncreaseLevelBy(chosen.levelIncrease);

        rewardUI.SetActive(false);
        Time.timeScale = 1f; // resume game
    }
}
