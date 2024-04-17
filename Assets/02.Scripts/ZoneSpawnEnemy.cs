using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSpawnEnemy : MonoBehaviour
{
    //player has two collider so i gotta count player collider
    public List<FixedEnemySpawn> spawnList = new List<FixedEnemySpawn>();
    public bool onlySpawn = false;

    public int collisionCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collisionCount++;
            if (collisionCount > 1)
                return;
            foreach (FixedEnemySpawn spawn in spawnList)
            {
                spawn.Spawn();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(onlySpawn)
            return;
        if (collision.CompareTag("Player"))
        {
            collisionCount--;
            if(collisionCount == 0)
            {
                foreach (FixedEnemySpawn spawn in spawnList)
                {
                    spawn.DeSpawn();
                }
            }
        }
    }
}
