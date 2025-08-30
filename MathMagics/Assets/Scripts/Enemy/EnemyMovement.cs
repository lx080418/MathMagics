using UnityEngine;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    public enum BehaviorMode { Sentry, Chase, Automatic }
    private enum InternalMode { Sentry, Chase }

    [Header("Behavior")]
    public BehaviorMode mode = BehaviorMode.Automatic;
    public Transform player;
    [SerializeField] private float chaseRadius = 5f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    [SerializeField] private Vector2 collisionBoxSize = Vector2.one * 0.8f;

    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveProgress = 0f;
    private Vector3 lastDirection = Vector3.zero;

    private readonly Vector3[] directions = {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right
    };

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;       
    }

    private void Start()
    {
        TurnManager.Instance.OnPlayerTurnEnded += TakeTurn;
    }

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
                TurnManager.Instance.BeginPlayerTurn();
            }
        }
    }

    InternalMode GetCurrentBehavior()
    {
        if (mode == BehaviorMode.Automatic && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            return (distanceToPlayer <= chaseRadius) ? InternalMode.Chase : InternalMode.Sentry;
        }

        return (InternalMode)System.Enum.Parse(typeof(InternalMode), mode.ToString());
    }

    void MoveRandomly()
    {
        Vector3 direction = directions[Random.Range(0, directions.Length)];
        TryMove(direction);
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return;

        List<Vector3> path = GridPathfinding.FindPath(transform, player.position);
        if (path != null && path.Count > 1)
        {
            print($"Enemy moving to {path[1]}");
            Vector3 nextStep = path[1]; // path[0] is current position
            Vector3 direction = (nextStep - transform.position).normalized;
            TryMove(direction);
        }
    }

    void TryMove(Vector3 direction)
    {
        Vector3 nextPosition = transform.position + direction;
        lastDirection = direction;

        Collider2D hit = Physics2D.OverlapBox(nextPosition, collisionBoxSize, 0f);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = GetCurrentBehavior() == InternalMode.Chase ? Color.yellow : Color.red;
            Gizmos.DrawWireCube(transform.position + lastDirection, collisionBoxSize);
        }
    }

    private void TakeTurn()
    {
        InternalMode actualMode = GetCurrentBehavior();
        switch (actualMode)
        {
            case InternalMode.Sentry:
                //MoveRandomly();
                break;
            case InternalMode.Chase:
                MoveTowardsPlayer();
                break;
        }
        TurnManager.Instance.BeginPlayerTurn();
    }
}
