using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//태그 검사후 충돌시 무적시간 잠시 넣어주고 맞을떄마다 
public class Destructable : MonoBehaviour
{
    public Animator _animator;
    public bool isDestructed = false;
    public string[] _damagableTag;
    public int coinSpawn = 0;

    public CombatComponent combat;



    private void Awake()
    {
        foreach (string tag in _damagableTag)
        {
            Assert.IsTrue(tag != "");
        }
        //파괴 처리
        combat.Init(transform);
        combat.OnDead += Destruct;
        combat.OnDamaged += Hit;

    }
    private void Hit()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Hit");
        }
    }
    public void Destruct()
    {
        PooledObjectSpawnManager.Instance.SpawnMoney(transform.position + Vector3.up * .4f, coinSpawn);
        if (_animator != null)
        {
            _animator.SetTrigger("Dead");
        }
        else
        {
            gameObject.SetActive(false);
        }
        isDestructed = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //파괴
        if (combat.IsDead())
        { return; }
        //공격했을때
        //damagableTag는 무조건 IDamageable 보장해야함
        foreach (string tag in _damagableTag)
        {
            //충돌체중 태그가 맞는게 있다면
            if (collision.CompareTag(tag))
            {
                //힛 처리
                combat.TakeDamage(collision.transform.position , 1f);
            }
        }
    }

}
