using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab:MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] FalseKnight Owner;
    [SerializeField] bool bothSide = false;
    public void OnEnable()
    {
        GameObject cur = Instantiate(prefab,transform.position,transform.rotation);
        if(bothSide)
        {
            GameObject cur2 = Instantiate(prefab, transform.position,Quaternion.Euler(0f,transform.rotation.eulerAngles.y + 180f,0f));
            cur2.GetComponent<DamageTrigger>().SetOwner(Owner);
        }
        cur.GetComponent<DamageTrigger>().SetOwner(Owner);
    }
}
