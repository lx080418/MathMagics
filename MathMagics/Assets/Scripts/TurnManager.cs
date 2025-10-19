using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [SerializeField] private PlayerInput playerInput;
    private bool isPlayerTurn = true;
    public event Action OnPlayerTurnEnded;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BeginPlayerTurn();
        Debug.Log("Player turn started.");
    }

    public void BeginPlayerTurn()
    {
        Debug.Log("Beginning player's turn.");
        isPlayerTurn = true;
        UpdateInputState();
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    public void EndPlayerTurn()
    {
        Debug.Log("Ending player's turn.");
        isPlayerTurn = false;
        UpdateInputState();
        
        OnPlayerTurnEnded?.Invoke();
    }

    private void UpdateInputState()
    {
        if (playerInput != null)
            playerInput.enabled = isPlayerTurn;
    }
}
