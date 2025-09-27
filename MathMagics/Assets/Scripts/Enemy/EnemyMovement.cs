using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using System.Runtime.CompilerServices;
using System.Collections;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;

public class EnemyMovement : MonoBehaviour
{
    public enum BehaviorMode { Sentry, Chase, Automatic, Attack, PreAttack }
    private enum InternalMode { Sentry, Chase, Attack, PreAttack }

    [Header("Behavior")]
    public BehaviorMode mode = BehaviorMode.Automatic;
    public Transform player;
    [SerializeField] private float chaseRadius = 5f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    [SerializeField] protected Vector2 collisionBoxSize = Vector2.one * 0.8f;
    [SerializeField] private float bumpAttackTime;
    [SerializeField] private GameObject exclamationMark;

    [Header("Animation")]
    [SerializeField] protected Animator _ac;
    [SerializeField] protected GameObject spriteObject;

    protected bool isMoving = false;
    protected Vector3 startPosition;
    protected Vector3 targetPosition;
    protected float moveProgress = 0f;
    protected Vector3 lastDirection = Vector3.zero;
    protected bool isPreAttacking = false;
    protected bool isTakingTurn;

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
        _ac.SetTrigger("Idle");
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnPlayerTurnEnded -= TakeTurn;
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
                isTakingTurn = false;
                _ac.SetBool("isWalking", false);
                TurnManager.Instance.BeginPlayerTurn();
            }
        }
    }

    InternalMode GetCurrentBehavior()
    {
        if (mode == BehaviorMode.Automatic && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            Debug.Log(distanceToPlayer);
            if (isPreAttacking)
            {
                return InternalMode.Attack;
            }
            else if (distanceToPlayer <= 1.7)
            {
                Debug.Log("Attack");
                return InternalMode.PreAttack;
            }
            else if (distanceToPlayer <= chaseRadius)
            {
                Debug.Log("Chase");
                return InternalMode.Chase;
            }
            else
            {
                Debug.Log("Sentry");
                return InternalMode.Sentry;
            }

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
        else
        {
            isTakingTurn = false;
        }
    }

    public virtual void TryMove(Vector3 direction)
    {
        Vector3 nextPosition = transform.position + direction;
        lastDirection = direction;

        Collider2D hit = Physics2D.OverlapBox(new Vector3(nextPosition.x + .5f, nextPosition.y+.5f, 0), collisionBoxSize, 0f);
        if (hit != null && (hit.CompareTag("Wall") || hit.CompareTag("Player") || hit.CompareTag("Enemy")))
        {
            isTakingTurn = false;
            return;
        }

        startPosition = transform.position;
        targetPosition = new Vector3(nextPosition.x, nextPosition.y, transform.position.z);
        moveProgress = 0f;
        _ac.SetBool("isWalking", true);
        if (targetPosition.x < startPosition.x)
        {
            spriteObject.transform.localScale = new Vector3(Mathf.Abs(spriteObject.transform.localScale.x) * -1, spriteObject.transform.localScale.y);
        }
        else if (targetPosition.x > startPosition.x)
        {
            spriteObject.transform.localScale = new Vector3(Mathf.Abs(spriteObject.transform.localScale.x), spriteObject.transform.localScale.y);
        }
        isMoving = true;
    }

    private void PreAttack()
    {
        isPreAttacking = true;
        exclamationMark.SetActive(true);
        isTakingTurn = false;
    }
    private void Attack()
    {
        //Get player's location
        if (player == null) return;

        StartCoroutine(BumpAttack());
    }

    private IEnumerator BumpAttack()
    {
        Vector3 targetPos = player.position;
        Vector3 originPosition = transform.position;
        Vector3 dir = (targetPos - originPosition).normalized;
        float elapsed = 0f;
        _ac.SetBool("isRolling", true);
        while (elapsed <= bumpAttackTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bumpAttackTime;

            transform.position = Vector3.Lerp(originPosition, originPosition + dir, t);
            yield return null;
        }
        transform.position = originPosition + dir;

        elapsed = 0f;
        while (elapsed <= bumpAttackTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bumpAttackTime;

            transform.position = Vector3.Lerp(originPosition + dir, originPosition, t);
            yield return null;
        }
        _ac.SetBool("isRolling", false);
        transform.position = originPosition;

        exclamationMark.SetActive(false);
        isPreAttacking = false;
        isTakingTurn = false;

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

    public void TakeTurn()
    {
        if (isTakingTurn) return;
        isTakingTurn = true;
        InternalMode actualMode = GetCurrentBehavior();
        switch (actualMode)
        {
            case InternalMode.Sentry:
                //MoveRandomly();
                isTakingTurn = false;
                break;
            case InternalMode.Chase:
                MoveTowardsPlayer();
                break;
            case InternalMode.Attack:
                Attack();
                break;
            case InternalMode.PreAttack:
                PreAttack();
                break;
        }
        TurnManager.Instance.BeginPlayerTurn();
    }
}
