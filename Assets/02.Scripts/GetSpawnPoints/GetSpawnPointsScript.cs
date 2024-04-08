using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSpawnPointsScript : MonoBehaviour
{
    public void SetSpawnPoints()
    {
        Collider2D col = GetComponent<Collider2D>();
        List<Collider2D> l = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter();
        Physics2D.OverlapCollider(col, filter, l);
        foreach (Collider2D c in l)
        {
            FixedEnemySpawn f = c.transform.GetComponent<FixedEnemySpawn>();
            if(f != null)
            {
                GetComponent<ZoneSpawnEnemy>().spawnList.Add(f);
            }
        }
    }
}
