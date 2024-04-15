using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;


public enum DamageType
{
    FixedDamage, StatPostDamage
}

public interface IFightable
{
    float GetHp();

    //public void TakeDamage(float damage);
    void TakeDamage(float damage, Vector2 Attackerpos);
    void DealFixedDamage(IFightable target, float damage);
    void DealDamage(IFightable target, float damage);

}
//Todo SetValues By Owner
public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private List<string> _damagableTag = new List<string>();
    [SerializeField] private Character _owner;
    [SerializeField] private DamageType _damageType;
    [SerializeField] private float _damage;

    internal void SetOwner(FalseKnight owner)
    {
        _owner = owner;
    }

    private void Awake()
    {
        foreach (string tag in _damagableTag) 
        { 
            Assert.IsTrue(tag != "");
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //����������
        //damagableTag�� ������ IDamageable �����ؾ���
        foreach(string tag in _damagableTag)
        {
            //�浹ü�� �±װ� �´°� �ִٸ�
            if (collision.CompareTag(tag))
            {
                //������ �������̽� 
                //�ٵ� �׳� �����Լ��ȿ��� ĳ�����ϰ� �θ��� ���� ���ѳ���
                IFightable target = collision.gameObject.GetComponent<IFightable>();
                switch (_damageType)
                {
                    case DamageType.FixedDamage:
                        ((IFightable)_owner).DealFixedDamage(target, _damage);
                        break;
                    //���� ��� �� ����
                    case DamageType.StatPostDamage:
                        ((IFightable)_owner).DealDamage(target, _damage);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
