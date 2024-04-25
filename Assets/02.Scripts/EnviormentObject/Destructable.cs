using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//�±� �˻��� �浹�� �����ð� ��� �־��ְ� ���������� 
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
        //�ı� ó��
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
        //�ı�
        if (combat.IsDead())
        { return; }
        //����������
        //damagableTag�� ������ IDamageable �����ؾ���
        foreach (string tag in _damagableTag)
        {
            //�浹ü�� �±װ� �´°� �ִٸ�
            if (collision.CompareTag(tag))
            {
                //�� ó��
                combat.TakeDamage(collision.transform.position , 1f);
            }
        }
    }

}
