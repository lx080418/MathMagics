using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool testingMode = false;
    [SerializeField] private Vector2 collisionBoxSize = Vector2.one * 0.8f;

    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveProgress = 0f;

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
        Debug.Log($"[PlayerMovement] TryMove called with direction: {direction}");

        if (isMoving)
            return;

        Vector3 proposedPosition = transform.position + new Vector3(direction.x, direction.y, 0f);
        Debug.Log($"{proposedPosition} with size {collisionBoxSize}");
        // Wall check using OverlapBox
        Collider2D hit = Physics2D.OverlapBox(new Vector3(proposedPosition.x+.5f, proposedPosition.y+.5f), collisionBoxSize, 0f);
        if (hit != null && (hit.CompareTag("Wall") || hit.CompareTag("Player")))
        {
            Debug.Log("Move blocked by wall or another player.");
            return;
        }
        else
        {
            Debug.Log($"No collider hit at {proposedPosition}");
        }

        Debug.Log("Move allowed");
        startPosition = transform.position;
        targetPosition = proposedPosition;
        isMoving = true;
        moveProgress = 0f;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void ForceMoveTo(Vector2 targetPosition, float speed)
    {
        if (isMoving) return;
        StartCoroutine(ForceMoveRoutine(targetPosition, speed));
    }

    private IEnumerator ForceMoveRoutine(Vector2 target, float speed)
    {
        Vector3 start = transform.position;
        float t = 0f;

        isMoving = true;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(targetPosition, collisionBoxSize);
    }
}
