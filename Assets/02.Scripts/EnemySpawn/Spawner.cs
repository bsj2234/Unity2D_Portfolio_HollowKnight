using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject _enemyPrefab;
    public GameObject _intancedEnemy;
    public virtual void Spawn()
    {
        throw new NotImplementedException();
    }
    public virtual void DeSpawn()
    {
        throw new NotImplementedException();
    }
}
