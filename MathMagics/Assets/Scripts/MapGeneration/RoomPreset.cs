using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPreset : MonoBehaviour
{
    public int roomWidth;
    public int roomHeight;
    public List<GameObject> enemies = new List<GameObject>();
    public List<Vector2> enemySpawnPositions = new();
    public GameObject enemyPrefab;
    public int maxNumEnemies = 2;


    private void Start()
    {
        SpawnEnemies();       
    }
    
    public void SpawnEnemies()
    {
        if (enemyPrefab == null || maxNumEnemies == 0 || enemySpawnPositions.Count == 0) return;
        List<Vector2> tempList = enemySpawnPositions;
        int numSpawns = Random.Range(1, maxNumEnemies + 1);
        for (int i = 0; i < numSpawns; i++)
        {
            //Spawn an enemy
            Vector2 randomLocation = tempList[Random.Range(0, tempList.Count)];

            GameObject enemy = Instantiate(enemyPrefab, transform);
            enemy.transform.localPosition = randomLocation;
            tempList.Remove(randomLocation);
        }
    }
}
