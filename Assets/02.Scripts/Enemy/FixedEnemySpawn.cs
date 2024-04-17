using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//controled bu zone
public class FixedEnemySpawn : MonoBehaviour
{

    //zone ���� �������� �������ִٰ� 
    public GameObject Enemy;
    public GameObject _intancedEnemy;



    public void Spawn()
    {
        _intancedEnemy = Instantiate(Enemy, transform.position, transform.rotation);
    }
    public void DeSpawn()
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