
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabOnEnable: MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] FalseKnight Owner;
    [SerializeField] bool bothSide = false;
    private GameObject cur = null;
    private GameObject cur2 = null;

    public void OnEnable()
    {
        cur = Instantiate(prefab,transform.position,transform.rotation);
        if(bothSide)
        {
            cur2 = Instantiate(prefab, transform.position,Quaternion.Euler(0f,transform.rotation.eulerAngles.y + 180f,0f));
            cur2.GetComponent<DamageTrigger>().SetOwner(Owner);
        }
        cur.GetComponent<DamageTrigger>().SetOwner(Owner);

        Owner.OnRealDead += DestroyProjectile;

    }

    private void OnDestroy()
    {
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        Destroy(cur);
        Destroy(cur2);
    }
}
