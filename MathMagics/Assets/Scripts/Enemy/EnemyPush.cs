using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPush : MonoBehaviour
{
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int knockbackTiles = 2;
    [SerializeField] private float knockbackSpeed = 10f;
    [SerializeField] private LayerMask playerLayer;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            PushPlayerBack();
        }
    }

    private void PushPlayerBack()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 knockbackTarget = (Vector2)player.position + direction * knockbackTiles;

        // Option 1: Smooth knockback using coroutine
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ForceMoveTo(knockbackTarget, knockbackSpeed);
        }

        // Option 2 (simpler): Instantly move the player (optional)
        // player.position = knockbackTarget;
    }
}
