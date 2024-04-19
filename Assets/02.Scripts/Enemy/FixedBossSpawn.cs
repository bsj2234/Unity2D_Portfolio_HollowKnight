using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

//controled bu zone
public class FixedBossSpawn : Spawner
{

    //zone 에서 스폰들을 가지고있다고 
    public Transform[] jumpTargets;

    public override void Spawn()
    {
        _intancedEnemy = Instantiate(_enemyPrefab, transform.position, transform.rotation);
        _intancedEnemy.transform.GetComponent<FalseKnight>().SetJumpTarget(jumpTargets);
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