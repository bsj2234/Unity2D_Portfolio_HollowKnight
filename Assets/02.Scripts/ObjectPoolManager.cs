using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    public GameObject PoolingObject;
    public int AddCount;

    private void Awake()
    {
        Assert.IsNotNull(PoolingObject, "PoolingObject is null");
        AddPool(AddCount);
    }

    void AddPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            pool.Enqueue(Instantiate(PoolingObject));
        }
    }

    public GameObject GetPoolingObject()
    {
        if(pool.Count == 0)
        {
            AddPool(AddCount);
        }
        return pool.Dequeue();
    }

    public void ReturnPoolingObject(GameObject gameObject)
    {
        pool.Enqueue(gameObject);
    }

}
