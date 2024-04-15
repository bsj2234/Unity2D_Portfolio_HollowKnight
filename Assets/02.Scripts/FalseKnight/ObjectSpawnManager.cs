using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[System.Serializable]
public class ObjectSpawnManager:Singleton<ObjectSpawnManager>
{
    private Dictionary<GameObject,ObjectPoolManager> _poolManagers = new Dictionary<GameObject, ObjectPoolManager>();

    public GameObject PoolManager;

    //등록할 오브젝트들 //같은 인덱스로 되어있음
    //더 유연하게 바꾸자
    [SerializeField] private GameObject Money;
    private void initPool(GameObject prefab)
    {
        if (!_poolManagers.ContainsKey(prefab))
        {
            _poolManagers[prefab] = Instantiate(PoolManager).GetComponent<ObjectPoolManager>();
            _poolManagers[prefab].PoolingObject = prefab;
        }
    }
    public void SpawnBetween(GameObject prefab, Vector2 attackerPos, Vector2 damagedPos, float time = -1)
    {
        if(!_poolManagers.ContainsKey(prefab))
        {
            initPool(prefab);
        }

        Vector2 midPos = (attackerPos + damagedPos) / 2;
        Vector2 dirAttackToTarget = (-attackerPos + damagedPos).normalized;
        float lookAngle = Vector2.Angle(dirAttackToTarget, Vector2.right);
        Vector3 LookRot = new Vector3(0f, 0f, -lookAngle);
        if(time == -1f)
        {
            _poolManagers[prefab].GetPoolingObject(midPos, LookRot);
        }
        else
        {
            _poolManagers[prefab].GetPoolingObjectWithTimer(midPos, LookRot, time);
        }
    }
    public void SpawnBetween(GameObject[] prefabs, Vector2 attackerPos, Vector2 damagedPos, float time = -1f)
    {
        foreach (GameObject prefab in prefabs)
        {

            SpawnBetween(prefab, attackerPos, damagedPos,time);
        }
    }
    //돈은 변화가 없으니 여기에 생성
    public void SpawnMoney(Vector3 pos, int count)
    {
        StartCoroutine(SpawnMoneyCoroutine(pos, count));
    }

    private IEnumerator SpawnMoneyCoroutine(Vector3 pos, int count)
    {
        if (!_poolManagers.ContainsKey(Money))
        {
            initPool(Money);
        }
        for (int i = 0; i < count; i++)
        {
            yield return null;
            _poolManagers[Money].GetPoolingObjectWithTimer(pos, 30f);
        }
        yield break;
    }
    public void ReturnMoney(GameObject obj)
    {
        _poolManagers[Money].ReturnPoolingObject(obj);
    }


    public void ReturnObject(GameObject prefab, GameObject obj)
    {
        _poolManagers[prefab].ReturnPoolingObject(obj);
    }
}