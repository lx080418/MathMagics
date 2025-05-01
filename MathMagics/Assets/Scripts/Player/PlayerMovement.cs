using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool testingMode = false; // ← Toggle this in Inspector
    private const string WALL_TAG = "Wall";

    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveProgress = 0f;
    private Tilemap wallTilemap;

    void Start()
    {
        GameObject wallObject = GameObject.FindGameObjectWithTag(WALL_TAG);
        if (wallObject != null)
        {
            wallTilemap = wallObject.GetComponent<Tilemap>();
        }

        if (wallTilemap == null)
        {
            Debug.LogError("Wall tilemap not found! Make sure the tilemap is tagged 'Wall'.");
        }
    }

    void OnEnable()
    {
        PlayerInput.OnMoveInput += TryMove;
    }

    void OnDisable()
    {
        PlayerInput.OnMoveInput -= TryMove;
    }

    void Update()
    {
        // Skip turn check if testing mode is enabled
        if (!testingMode && (!TurnManager.Instance || !TurnManager.Instance.IsPlayerTurn()))
            return;

        if (!isMoving)
            return;

        moveProgress += Time.deltaTime * moveSpeed;
        moveProgress = Mathf.Min(moveProgress, 1f);
        transform.position = Vector3.Lerp(startPosition, targetPosition, moveProgress);

        if (moveProgress >= 1f)
        {
            isMoving = false;
            moveProgress = 0f;

            if (!testingMode)
            {
                TurnManager.Instance.EndPlayerTurn();
            }
        }
    }

    public void TryMove(Vector2 direction)
    {
        if (isMoving || wallTilemap == null)
            return;

        Vector3 proposedPosition = transform.position + new Vector3(direction.x, direction.y, 0f);
        Vector3Int cell = wallTilemap.WorldToCell(proposedPosition);

        if (wallTilemap.GetTile(cell) == null)
        {
            startPosition = transform.position;
            targetPosition = proposedPosition;
            isMoving = true;
            moveProgress = 0f;
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
