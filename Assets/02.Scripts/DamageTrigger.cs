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
        //공격했을때
        //damagableTag는 무조건 IDamageable 보장해야함
        foreach(string tag in _damagableTag)
        {
            //충돌체중 태그가 맞는게 있다면
            if (collision.CompareTag(tag))
            {
                //데미지 인터페이스 
                //근데 그냥 어택함수안에서 캐스팅하고 부르자 응집 시켜놓자
                IFightable target = collision.gameObject.GetComponent<IFightable>();
                switch (_damageType)
                {
                    case DamageType.FixedDamage:
                        ((IFightable)_owner).DealFixedDamage(target, _damage);
                        break;
                    //스탯 계산 후 공격
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
