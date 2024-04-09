using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObjectPoolManager<T> : Singleton<ObjectPoolManager<T>> where T : Object
{
    private Queue<T> pool = new Queue<T>();
    public GameObject PoolingObject;
    public int AddCount;
    public float NotUsedTime = 0f;

    private void Awake()
    {
        Assert.IsNotNull(PoolingObject, "PoolingObject is null");
        if(typeof(T).IsSubclassOf(typeof(GameObject)) == false) // 인텔리센스 넌 대체..
        {
            Assert.IsTrue(false, " Not a Prefab Type");
        }
    }

    void AddPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            pool.Enqueue(Instantiate(PoolingObject) as T);
        }
    }

    public T GetPoolingObject()
    {
        if(pool.Count == 0)
        {
            AddPool(AddCount);
        }
        return pool.Dequeue();
    }

    public void ReturnPoolingObject(T gameObject)
    {
        pool.Enqueue(gameObject as T);
    }
}
