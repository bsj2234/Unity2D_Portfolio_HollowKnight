using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FalseKnightMainBody : MonoBehaviour, IFightable
{
    public CombatComponent combatComponent;
    public Action OnDead;

    private Animator _animator;
    public GameObject[] damagedEffects;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Assert.IsNotNull(_animator);
        Assert.IsNotNull(combatComponent);
    }

    public void ResetMainBody()
    {
        combatComponent.ResetHp();
    }
    public void Spawn()
    {
        ResetMainBody();
        gameObject.SetActive(true);
    }

    float IFightable.GetHp()
    {
        return combatComponent.GetHp();
    }

    void IFightable.TakeDamage(float damage, Vector2 attackerPos)
    {
        combatComponent.TakeDamage(damage);
        _animator.SetTrigger("Hit");
        ObjectSpawnManager.Instance.SpawnBetween(damagedEffects, attackerPos, transform.position, 4f);
        if (combatComponent.IsDead())
        {
            if (OnDead != null)
            {
                OnDead.Invoke();
            }
            gameObject.SetActive(false);
        }
    }

    void IFightable.DealFixedDamage(IFightable target, float damage)
    {
    }

    void IFightable.DealDamage(IFightable target, float damage)
    {
    }
}
