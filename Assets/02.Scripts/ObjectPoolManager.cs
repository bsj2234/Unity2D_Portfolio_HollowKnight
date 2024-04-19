using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObjectPoolManager:MonoBehaviour
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    public GameObject PoolingObject;
    public int AddCount = 5;
    public float NotUsedGameObjectime = 0f;


    private void AddPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(PoolingObject);
            temp.SetActive(false);
            pool.Enqueue(temp);
        }
    }

    public GameObject GetPoolingObject()
    {
        if (pool.Count == 0)
        {
            AddPool(AddCount);
        }
        GameObject temp = pool.Dequeue();
        temp.SetActive(true);
        return temp;
    }
    public GameObject GetPoolingObject(Vector3 position)
    {
        GameObject temp = GetPoolingObject();
        temp.transform.position = position;
        return temp;
    }
    public GameObject GetPoolingObject(Vector3 position, Vector3 rotation, float scale = -1f)
    {
        GameObject temp = GetPoolingObject();
        temp.transform.position = position;
        temp.transform.rotation = Quaternion.Euler(rotation);
        if (scale > 0f)
        {
            temp.transform.localScale = new Vector3(scale, scale, scale);
        }
        return temp;
    }
    //만들고 생명을 시간으로 정해줌
    public GameObject GetPoolingObjectWithTimer(float time)
    {
        GameObject temp = GetPoolingObject();
        StartCoroutine(ReturnTime(temp, time));
        return temp;
    }
    public GameObject GetPoolingObjectWithTimer(Vector3 position, float time)
    {
        GameObject temp = GetPoolingObject(position);
        StartCoroutine(ReturnTime(temp, time));
        return temp;
    }
    public GameObject GetPoolingObjectWithTimer(Vector3 position, Vector3 rotation, float scale, float time)
    {
        GameObject temp = GetPoolingObject(position, rotation, scale);
        StartCoroutine(ReturnTime(temp, time));
        return temp;
    }
    public IEnumerator ReturnTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        ReturnPoolingObject(obj);
    }

    public void ReturnPoolingObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
        pool.Enqueue(gameObject);
    }
}
