using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : EnemyMovement
{
    public override void TryMove(Vector3 direction)
    {
        Vector3 nextPosition = transform.position + direction;
        lastDirection = direction;
        Collider2D hit;
        //If we're moving right, need to raycast a bit further since origin point is bottom left
        //Right
        if (nextPosition.x > transform.position.x)
        {
            Debug.Log($"Boss moving Right {nextPosition}");
            hit = Physics2D.OverlapBox(new Vector3(nextPosition.x + 1.5f, nextPosition.y + 1f, 0), collisionBoxSize, 0f);
        }
        //Left
        else if (nextPosition.x < transform.position.x)
        {
            Debug.Log($"Boss moving Left {nextPosition}");
            hit = Physics2D.OverlapBox(new Vector3(nextPosition.x + .5f, nextPosition.y + 1f, 0), collisionBoxSize, 0f);
        }
        //Up
        else if (nextPosition.y > transform.position.y)
        {
            Debug.Log($"Boss moving Up {nextPosition}");
            hit = Physics2D.OverlapBox(new Vector3(nextPosition.x + 1, nextPosition.y + 1.5f, 0), collisionBoxSize, 0f);
        }
        //Down
        else
        {
            Debug.Log($"Boss moving Down {nextPosition}");
            hit = Physics2D.OverlapBox(new Vector3(nextPosition.x + 1, nextPosition.y + .5f, 0), collisionBoxSize, 0f);
        }

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
        isTakingTurn = false;
    }
    public override void MoveTowardsPlayer()
    {
        if (player == null) return;

        List<Vector3> path = GridPathfinding.FindPath(transform, player.position, pathWidth:2);
        if (path != null && path.Count > 1)
        {
            print($"Boss moving to {path[1]}");
            Vector3 nextStep = path[1]; // path[0] is current position
            Vector3 direction = (nextStep - transform.position).normalized;
            TryMove(direction);
        }
        else
        {
            isTakingTurn = false;
        }
    }
}
