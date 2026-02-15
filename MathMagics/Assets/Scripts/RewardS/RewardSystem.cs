using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Instance;

    [Header("UI")]
    public GameObject rewardUI;
    public RewardCard[] rewardCards;
    public GameObject rewardOutline;
    

    [Header("Audio")]
    [SerializeField] private AudioClip rewardSFX;



    //* -------- Internal -------- */
    private int hoveredReward = 0;
    private List<RewardOption> rewardPool = new();
    private bool isShowingRewards = false;

    // -------- Events -------
    public event Action OnPotionHitRewardSelected;
    public event Action OnPotionHitConfirmed;


    private PlayerPotion playerPotion;


    private Dictionary<int, Dictionary<string, float>> weaponDropChances = new Dictionary<int, Dictionary<string, float>>()
    {
        { 1, new Dictionary<string, float>
            {
                {"Subtraction", 1.0f},
                {"Addition", 0.0f},
                {"Multiplication", 0.0f},
                {"Division", 0.0f}
            }
        },
                { 2, new Dictionary<string, float>
            {
                {"Subtraction", .3f},
                {"Addition", 0.7f},
                {"Multiplication", 0.0f},
                {"Division", 0.0f}
            }
        },
                { 3, new Dictionary<string, float>
            {
                {"Subtraction", 0.3f},
                {"Addition", 0.3f},
                {"Multiplication", 0.4f},
                {"Division", 0.0f}
            }
        },
                { 4, new Dictionary<string, float>
            {
                {"Subtraction", 0.2f},
                {"Addition", 0.2f},
                {"Multiplication", 0.2f},
                {"Division", 0.4f}
            }
        },


    };

    private void Awake()
    {
        Instance = this;
        
    }

    void Start()
    {
        rewardUI.SetActive(false);
        
        rewardOutline.SetActive(false);
        playerPotion = FindObjectOfType<PlayerPotion>();
        PlayerInput.OnMoveInput += HandlePlayerMoveInput;
        PlayerInput.OnSpaceInput += HandlePlayerSpaceInput;
    }

    private void OnDestroy()
    {
        PlayerInput.OnMoveInput -= HandlePlayerMoveInput;
        PlayerInput.OnSpaceInput -= HandlePlayerSpaceInput;
    }



    private void HandlePlayerMoveInput(Vector2 vector)
    {
        if(!isShowingRewards) return;
        hoveredReward += (int)vector.x;
        hoveredReward = Mathf.Clamp(hoveredReward,0,2);
        HighlightRewardCard(hoveredReward);
    }

    private void HandlePlayerSpaceInput()
    {
        if(!isShowingRewards) return;
        SelectReward(hoveredReward);
    }

    private void HighlightRewardCard(int index)
    {
        rewardOutline.transform.position = rewardCards[index].transform.position;
    }


    public void ShowRewardOptions()
    {
        Time.timeScale = 0f;
        PlayerInput.LockMovement();
        isShowingRewards = true;
        rewardUI.SetActive(true);
        
        
        hoveredReward = 0;
        GenerateRewardPool();

        for (int i = 0; i < rewardCards.Length; i++)
        {
            RewardOption option = rewardPool[i];
            rewardCards[i].Initialize(option);
        }
        rewardOutline.SetActive(true);
        StartCoroutine(DelayHighlightOneFrame());
    }

    private IEnumerator DelayHighlightOneFrame()
    {
        yield return null;
        HighlightRewardCard(0);
    }

    private void GenerateRewardPool()
    {
        rewardPool.Clear();

        string[] weaponNames = new string[] { "Subtraction", "Addition", "Multiplication", "Division" };

        for (int i = 0; i < 3; i++)
        {
            if (UnityEngine.Random.value < 0.2f) // 25% chance to include a health reward
            {
                Rarity rarity = GetRandomRarity();
                int healthAmount = rarity switch
                {
                    Rarity.Common => 5,
                    Rarity.Rare => 10,
                    Rarity.Epic => 20, //This will do nothing now actually
                    _ => 5
                };

                PlayerPotion playerPotion = FindObjectOfType<PlayerPotion>();
                if (playerPotion != null && playerPotion.HasPotion())
                {
                    if(rarity != Rarity.Special)
                    {
                        rewardPool.Add(new RewardOption(
                            "Potion",
                            $"Upgrade Potion by {healthAmount}",
                            rarity,
                            healthAmount,
                            RewardType.Health
                        ));  
                    }
                    else
                    {
                        rewardPool.Add(new RewardOption(
                            "Potion",
                            $"Attack your health potion!",
                            rarity,
                            healthAmount,
                            RewardType.Health
                        ));  
                    }
                }
                else
                {
                    rewardPool.Add(new RewardOption(
                        "Potion",
                        $"Gain Potion healing {healthAmount} HP",
                        rarity,
                        healthAmount,
                        RewardType.Health
                    ));
                }
            }
            else
            {
                string chosenWeapon = GetRandomWeaponByDropChance();
                Rarity rarity = GetRandomRarity();
                while(rarity == Rarity.Special)
                {
                    rarity = GetRandomRarity();
                }
                int level = rarity switch
                {
                    Rarity.Common => 1,
                    Rarity.Rare => 2,
                    Rarity.Epic => 3,
                    _ => 1
                };

                rewardPool.Add(new RewardOption(chosenWeapon, $"Upgrade {chosenWeapon} by {level} level", rarity, level, RewardType.Weapon));
            }
        }
    }


    private Rarity GetRandomRarity()
    {
        float roll = UnityEngine.Random.value;
        if (roll < 0.4f) return Rarity.Common;
        if (roll < 0.7) return Rarity.Rare;
        if (roll < 0.9f) return Rarity.Epic;
        return Rarity.Special;
    }

    public void SelectReward(int index)
    {
        RewardOption chosen = rewardPool[index];

        Debug.Log($"[RewardSystem] Selected reward: {chosen.description}");

        AudioManager.Instance.PlayOneShot(rewardSFX, 1f, AudioManager.Instance.sfxAMG);

        if (chosen.rewardType == RewardType.Weapon)
        {
            WeaponHandler.Instance.GetWeaponByName(chosen.weaponName)
                ?.IncreaseLevelBy(chosen.levelIncrease);
        }
        else if (chosen.rewardType == RewardType.Health)
        {
            //If this is the Epic Health reward, do the whole combat potion thing
            if(chosen.rarity == Rarity.Special)
            {

                OnPotionHitRewardSelected?.Invoke();
                WeaponHandler.Instance.OnAttackPerformed += HandleAttackPerformed;
            }
            else
            {
                if (playerPotion != null)
                {
                    if (playerPotion.HasPotion())
                    {
                        playerPotion.ModifyPotionAmount("+" + chosen.levelIncrease.ToString());
                    }
                    else
                    {
                        playerPotion.SetPotion(chosen.levelIncrease.ToString());
                    }
                }
            }
        }
        rewardOutline.SetActive(false);
        rewardUI.SetActive(false);
        isShowingRewards = false;
         PlayerInput.UnlockMovement();
        Time.timeScale = 1f;
    }

    private void HandleAttackPerformed()
    {
        WeaponHandler.Instance.OnAttackPerformed -= HandleAttackPerformed;
        PlayerPotion.Instance.ModifyPotionAmount(WeaponHandler.Instance.GetCurrentWeapon().GetDamageExpression());
        OnPotionHitConfirmed?.Invoke();
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
        float roll = UnityEngine.Random.value * total;
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

public void ShowBossRewardOptions()
{
    Time.timeScale = 0f;
    rewardUI.SetActive(true);
    rewardOutline.SetActive(true);
    GenerateBossRewardPool();

    for (int i = 0; i < rewardCards.Length; i++)
    {
        RewardOption option = rewardPool[i];
        rewardCards[i].Initialize(option);
    }
    HighlightRewardCard(0);
}

private void GenerateBossRewardPool()
{
    rewardPool.Clear();

    string[] weaponNames = new string[] { "Subtraction", "Addition", "Multiply", "Divide" };

    for (int i = 0; i < 3; i++)
    {
        Rarity rarity = Rarity.Epic; // Always epic
        if(i == 1  && playerPotion != null && playerPotion.HasPotion())
        {
            rarity = Rarity.Special;
            rewardPool.Add(new RewardOption(
                "Potion",
                $"Attack your potion",
                rarity,
                0,
                RewardType.Health
            ));
            
        }
        else if (UnityEngine.Random.value < 0.2f) // 20% chance to offer health reward
        {
            int healthAmount = 15; // Epic value

            PlayerPotion playerPotion = FindObjectOfType<PlayerPotion>();
            if (playerPotion != null && playerPotion.HasPotion())
            {
                rewardPool.Add(new RewardOption(
                    "Potion",
                    $"Upgrade Potion (+{healthAmount} HP)",
                    rarity,
                    healthAmount,
                    RewardType.Health
                ));
            }
            else
            {
                rewardPool.Add(new RewardOption(
                    "Potion",
                    $"Gain Potion",
                    rarity,
                    healthAmount,
                    RewardType.Health
                ));
            }
        }
        else
        {
            string chosenWeapon = GetRandomWeaponByDropChance();
            int level = 3; // Epic level

            rewardPool.Add(new RewardOption(
                chosenWeapon,
                $"Upgrade {chosenWeapon} by {level} level",
                rarity,
                level,
                RewardType.Weapon
            ));
        }
    }
}




}
