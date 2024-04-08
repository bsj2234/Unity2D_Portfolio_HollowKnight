using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSpawnEnemy : MonoBehaviour
{
    public List<FixedEnemySpawn> spawnList = new List<FixedEnemySpawn>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            foreach (FixedEnemySpawn spawn in spawnList)
            {
                spawn.Spawn();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (FixedEnemySpawn spawn in spawnList)
            {
                spawn.DeSpawn();
            }
        }
    }
}
