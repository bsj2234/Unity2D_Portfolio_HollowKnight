using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FalseKnightMainBody : Character
{
    public CombatComponent combatComponent;
    public Action OnFalseKnightDead;

    private Animator _animator;
    public GameObject[] damagedEffects;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Assert.IsNotNull(_animator);
        Assert.IsNotNull(combatComponent);
        combatComponent.Init(transform);
        combatComponent.OnDamaged += Ondamaged;
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
    //interface
    public float GetHp()
    {
        return combatComponent.GetHp();
    }

    public void TakeDamage(float damage, Vector2 attackerPos)
    {
        combatComponent.TakeDamage(attackerPos, damage);
    }
    private void Ondamaged()
    {
        _animator.SetTrigger("Hit");
        gameObject.SetActive(false);
    }


    public void DealFixedDamage(IFightable target, float damage)
    {
    }

    public void DealDamage(IFightable target, float damage)
    {
    }

    public bool IsDead()
    {
        return combatComponent.IsDead();
    }

    public override CombatComponent GetCombatComponent()
    {
        return combatComponent;
    }
}
