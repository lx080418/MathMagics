using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BossRoomPreset : RoomPreset
{
    [Header("Portal")]
    public Vector3Int portalSpawnPosition;
    public GameObject portalPrefab;

    public override void SpawnEnemies()
    {
        if (enemyPrefab == null || maxNumEnemies == 0 || enemySpawnPositions.Count == 0) return;

        GameObject boss = Instantiate(enemyPrefab, transform);
        enemies.Add(boss);
        boss.transform.localPosition = enemySpawnPositions[0];
        Debug.Log("Subscribing to Boss On Death");
        boss.GetComponent<EnemyHealth>().OnEnemyDied += HandleBossDeath;
    }

    private void HandleBossDeath()
    {
        Debug.Log("Spawning Portal!");
        GameObject portal = Instantiate(portalPrefab, transform);
        portal.transform.localPosition = portalSpawnPosition;
        //Sound effect, portal.Initialize()
    }

    private void OnDestroy()
    {
        if(enemies.Count > 0)
        {
            enemies[0].GetComponent<EnemyHealth>().OnEnemyDied -= HandleBossDeath;
        }
        
    }
}
