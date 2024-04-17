using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//controled bu zone
public class FixedEnemySpawn : MonoBehaviour
{

    //zone ���� �������� �������ִٰ� 
    public GameObject Enemy;
    private GameObject _enemy;



    public void Spawn()
    {
        _enemy = Instantiate(Enemy, transform.position, transform.rotation);
    }
    public void DeSpawn()
    {
        if(_enemy != null)
        {
            Destroy(_enemy);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}