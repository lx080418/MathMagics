using UnityEngine;

public class EnemySentry : MonoBehaviour
{
    public enum BehaviorMode { Sentry, Chase }
    [Header("Behavior")]
    public BehaviorMode mode = BehaviorMode.Sentry;
    public Transform player;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float waitTimeBetweenMoves = 1f;

    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveProgress = 0f;
    private float waitTimer = 0f;
    private Vector3 lastDirection = Vector3.zero;

    private readonly Vector3[] directions = {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right
    };

    void Update()
    {
        if (isMoving)
        {
            moveProgress += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, moveProgress);

            if (moveProgress >= 1f)
            {
                transform.position = targetPosition;
                isMoving = false;
                waitTimer = waitTimeBetweenMoves;
            }
        }
        else
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                switch (mode)
                {
                    case BehaviorMode.Sentry:
                        MoveRandomly();
                        break;
                    case BehaviorMode.Chase:
                        MoveTowardsPlayer();
                        break;
                }
            }
        }
    }

    void MoveRandomly()
    {
        Vector3 direction = directions[Random.Range(0, directions.Length)];
        TryMove(direction);
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector3 bestDirection = Vector3.zero;
        float shortestDistance = float.MaxValue;

        foreach (Vector3 dir in directions)
        {
            Vector3 testPos = transform.position + dir;
            float dist = Vector3.Distance(testPos, player.position);

            // Check for collision
            Collider2D hit = Physics2D.OverlapBox(testPos, Vector2.one * 0.8f, 0f);
            if (hit != null && (hit.CompareTag("Wall") || hit.CompareTag("Player")))
                continue;

            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                bestDirection = dir;
            }
        }

        if (bestDirection != Vector3.zero)
        {
            TryMove(bestDirection);
        }
    }

    void TryMove(Vector3 direction)
    {
        Vector3 nextPosition = transform.position + direction;
        lastDirection = direction;

        Collider2D hit = Physics2D.OverlapBox(nextPosition, Vector2.one * 0.8f, 0f);

        if (hit != null && (hit.CompareTag("Wall") || hit.CompareTag("Player")))
        {
            return;
        }

        startPosition = transform.position;
        targetPosition = new Vector3(nextPosition.x, nextPosition.y, transform.position.z);
        moveProgress = 0f;
        isMoving = true;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = mode == BehaviorMode.Chase ? Color.yellow : Color.red;
        Gizmos.DrawWireCube(transform.position + lastDirection, Vector3.one * 0.8f);
    }
}
