using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//controled bu zone
public class FixedEnemySpawn : Spawner
{

    //zone ���� �������� �������ִٰ� 



    public override void Spawn()
    {
        _intancedEnemy = Instantiate(_enemyPrefab, transform.position, transform.rotation);
    }
    public override void DeSpawn()
    {
        if(_intancedEnemy != null)
        {
            Destroy(_intancedEnemy);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}