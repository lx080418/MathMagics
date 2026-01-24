using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoseScreenBrain : MonoBehaviour
{

    [SerializeField] private TMP_Text hitCounterText;
    [SerializeField] private GameObject loseScreen;

    private void Start()
    {
        PlayerHealth.instance.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDestroy()
    {
        PlayerHealth.instance.OnPlayerDeath -= HandlePlayerDeath;
    }


    private void HandlePlayerDeath()
    {
        hitCounterText.text = $"You've hit enemies <color=#db4646>{HitCounter.Instance.GetNumHits()}</color> times!";
        loseScreen.SetActive(true);
    }
}
