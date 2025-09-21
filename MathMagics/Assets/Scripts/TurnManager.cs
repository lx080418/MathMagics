using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [SerializeField] private PlayerInput playerInput;
    private bool isPlayerTurn = true;
    private bool hasActed = false;
    public event Action OnPlayerTurnEnded;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        PlayerInput.OnMoveInput += HandlePlayerAction;
        PlayerInput.OnAttackInput += HandlePlayerAction;
    }

    void OnDisable()
    {
        PlayerInput.OnMoveInput -= HandlePlayerAction;
        PlayerInput.OnAttackInput -= HandlePlayerAction;
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
        hasActed = false;
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
        // Add enemy turn logic here

        //Idea
        //Enemies in Chase mode one by one take their turns
        //enemies in Sentry mode take their turns all at once.
        print("Player Turn Ended!");
        //OnPlayerTurnEnded?.Invoke();

        //If there are no enemies, immediately start the player's turn again.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            BeginPlayerTurn();
        }
        else
        {
            StartCoroutine(TakeEnemyTurns(enemies));
        }

    }

    private IEnumerator TakeEnemyTurns(GameObject[] enemies)
    {
        foreach (GameObject go in enemies)
        {
            go.GetComponentInParent<EnemyMovement>().TakeTurn();
            yield return new WaitForSeconds(.2f);
        }
    }

    private void UpdateInputState()
    {
        if (playerInput != null)
            playerInput.enabled = isPlayerTurn;
    }

    private void HandlePlayerAction(Vector2 _) => TryEndTurn();
    private void HandlePlayerAction() => TryEndTurn();

    private void TryEndTurn()
    {
        // if (!hasActed)
        // {
        //     hasActed = true;
        //     EndPlayerTurn();
        // }
    }
}
