using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [SerializeField] private PlayerInput playerInput;
    private bool isPlayerTurn = true;
    private bool hasActed = false;

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
        isPlayerTurn = false;
        UpdateInputState();
        // Add enemy turn logic here
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
        if (!hasActed)
        {
            hasActed = true;
            //EndPlayerTurn();
        }
    }
}
