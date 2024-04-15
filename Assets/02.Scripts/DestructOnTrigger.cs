using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructOnTrigger : MonoBehaviour
{
    private bool destructed = false;

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(DestructTimer());
    }
    private IEnumerator DestructTimer()
    {
        yield return new WaitForSeconds(.5f);
        Destruct();
    }
    private void Destruct()
    {
        destructed = true;
        gameObject.SetActive(false);
    }
}
