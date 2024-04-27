using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObjectSpawnManager:Singleton<PooledObjectSpawnManager>
{
    private Dictionary<GameObject,ObjectPoolManager> _poolManagers = new Dictionary<GameObject, ObjectPoolManager>();

    [SerializeField]private GameObject PoolManagerPrefab;
    [SerializeField] private GameObject MoneyPrefab;
    public GameObject[] defaultHitEffect;

    private void initPool(GameObject prefab)
    {
        if (!_poolManagers.ContainsKey(prefab))
        {
            _poolManagers[prefab] = Instantiate(PoolManagerPrefab).GetComponent<ObjectPoolManager>();
            _poolManagers[prefab].PoolingObject = prefab;
        }
    }
    public void SpawnBetweenAttacker(GameObject[] prefabs, Vector2 attackerPos, Vector2 damagedPos, float scale,float time = -1f)
    {
        foreach (GameObject prefab in prefabs)
        {
            SpawnBetweenAttackerRandomRot(prefab, attackerPos, damagedPos, scale,time);
        }
    }
    public void SpawnBetweenAttackerRandomRot(GameObject prefab, Vector2 attackerPos, Vector2 damagedPos, float scale = -1f, float time = -1f, float addRandomRotation = 20f)
    {
        if (!_poolManagers.ContainsKey(prefab))
        {
            initPool(prefab);
        }

        Vector2 midPos = (attackerPos + damagedPos) / 2;
        Vector2 dirAttackToTarget = (-attackerPos + damagedPos).normalized;
        float lookAngle = Vector2.Angle(dirAttackToTarget, Vector2.right);
        lookAngle += Random.Range(-addRandomRotation, addRandomRotation);
        Vector3 LookRot = new Vector3(0f, 0f, -lookAngle);
        if (time == -1f)
        {
            _poolManagers[prefab].GetPoolingObject(midPos, LookRot, scale);
        }
        else
        {
            _poolManagers[prefab].GetPoolingObjectWithTimer(midPos, LookRot, scale, time);
        }
    }
    //돈은 변화가 없으니 여기에 생성
    public void SpawnMoney(Vector3 pos, int count)
    {
        StartCoroutine(SpawnMoneyCoroutine(pos, count));
    }
    private IEnumerator SpawnMoneyCoroutine(Vector3 pos, int count)
    {
        if (!_poolManagers.ContainsKey(MoneyPrefab))
        {
            initPool(MoneyPrefab);
        }
        for (int i = 0; i < count; i++)
        {
            yield return null;
            _poolManagers[MoneyPrefab].GetPoolingObjectWithTimer(pos, 30f);
        }
        yield break;
    }
    public void ReturnMoney(GameObject obj)
    {
        _poolManagers[MoneyPrefab].ReturnPoolingObject(obj);
    }
    internal void ReturnMoney(GameObject obj, float time)
    {
        _poolManagers[MoneyPrefab].ReturnPoolingObject(obj, time);
    }
    public void ReturnObject(GameObject prefab, GameObject obj)
    {
        _poolManagers[prefab].ReturnPoolingObject(obj);
    }
    internal void SpawnDefalutHitEffect(Vector3 attackerPos, Vector3 damagedPos, float scale = -1f, float time = 2f)
    {
        SpawnBetweenAttacker(defaultHitEffect, attackerPos, damagedPos, scale, time);
    }

}