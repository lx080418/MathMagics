using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
public class PotionUI : MonoBehaviour
{
    [SerializeField] private PlayerPotion playerPotion;
    public TMP_Text potionAmountText;
    public Image potionImage;
    public float moveTime;
    public GameObject potionParent;
    private void Awake()
    {
        playerPotion.OnPotionChanged += HandlePotionChanged;
        
    }

    private void Start()
    {
        RewardSystem.Instance.OnPotionHitRewardSelected += HandlePotionHitRewardSelected;
        RewardSystem.Instance.OnPotionHitConfirmed += HandlePotionHitConfirmed;
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

    private void HandlePotionHitRewardSelected()
    {
        //lerp the potion image to infront of the player.
        Debug.Log("Doing Potion Hit Reward!");
        StartCoroutine(DoPotionHitReward());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            HandlePotionHitRewardSelected();
        }
    }

    private IEnumerator DoPotionHitReward()
    {
        yield return StartCoroutine(LerpToPlayer());
    }

    private IEnumerator LerpToPlayer()
    {
        Vector3 startPos = potionImage.transform.position;
        Vector3 spawnDirection = new Vector3(PlayerInput.lastDirection.x, PlayerInput.lastDirection.y, 0f);
        Vector3 targetPos = Camera.main.WorldToScreenPoint(GameObject.Find("Player").transform.position + spawnDirection + new Vector3(.5f, .5f));
        float elapsed = 0f;

        while(elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed/moveTime;
            potionImage.transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        potionImage.transform.position = targetPos;
    }

    private void HandlePotionHitConfirmed()
    {
        StartCoroutine(DoPotionAfterHit());
    }

    private IEnumerator DoPotionAfterHit()
    {
        yield return StartCoroutine(LerpToSlot());
    }

    private IEnumerator LerpToSlot()
    {
        Vector3 startPos = potionImage.transform.position;
        Vector3 targetPos = potionParent.transform.position;
        float elapsed = 0f;

        while(elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed/moveTime;
            potionImage.transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        potionImage.transform.position = targetPos;
    }
}
