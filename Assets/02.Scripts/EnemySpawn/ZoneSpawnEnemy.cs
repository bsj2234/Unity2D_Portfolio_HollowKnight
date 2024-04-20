using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSpawnEnemy : MonoBehaviour
{
    //player has two collider so i gotta count player collider
    public List<Spawner> spawnList = new List<Spawner>();

    public int collisionCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collisionCount++;
            if (collisionCount > 1)
                return;
            foreach (Spawner spawn in spawnList)
            {
                spawn.Spawn();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collisionCount--;
            if(collisionCount == 0)
            {
                foreach (Spawner spawn in spawnList)
                {
                    spawn.DeSpawn();
                }
            }
        }
    }
}
