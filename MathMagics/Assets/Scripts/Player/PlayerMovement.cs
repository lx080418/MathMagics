using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool testingMode = false;
    [SerializeField] private Vector2 collisionBoxSize = Vector2.one * 0.8f;
    [SerializeField] private float moveTime;
    private bool isMoving = false;
    private Vector3 targetPosition;
    [Header("Audio")]
    [SerializeField] private AudioClip playerWalkSFX;

    void OnEnable()
    {
        PlayerInput.OnMoveInput += TryMove;
        GameManager.beatLevel += ResetPlayerPosition;
    }

    void OnDisable()
    {
        PlayerInput.OnMoveInput -= TryMove;
    }


    public void TryMove(Vector2 direction)
    {
        Debug.Log($"[PlayerMovement] TryMove called with direction: {direction}");

        if (isMoving)
            return;

        Vector3 proposedPosition = transform.position + new Vector3(direction.x, direction.y, 0f);
        Debug.Log($"{proposedPosition} with size {collisionBoxSize}");
        // Wall check using OverlapBox
        Collider2D hit = Physics2D.OverlapBox(new Vector3(proposedPosition.x + .5f, proposedPosition.y + .5f), collisionBoxSize, 0f);
        if (hit != null && (hit.CompareTag("Wall") || hit.CompareTag("Player") || hit.CompareTag("Enemy")))
        {
            Debug.Log("Move blocked by wall or another player.");
            return;
        }
        else
        {
            Debug.Log($"No collider hit at {proposedPosition}");
        }

        Debug.Log("Move allowed");
        targetPosition = proposedPosition;
        //isMoving = true;
        //moveProgress = 0f;
        StartCoroutine(MoveRoutine(targetPosition));
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

    private IEnumerator MoveRoutine(Vector2 target)
    {
        isMoving = true;
        AudioManager.Instance.PlayOneShotVariedPitch(playerWalkSFX, 1f, AudioManager.Instance.sfxAMG, .03f);
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed <= moveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveTime;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.position = target;
        isMoving = false;
        TurnManager.Instance.EndPlayerTurn();
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(targetPosition, collisionBoxSize);
    }

    private void ResetPlayerPosition(int level)
    {
        StopAllCoroutines();
        transform.position = Vector3.zero;
    }
}
