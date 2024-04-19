using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FalseKnightMainBody : Character
{
    public FalseKnight owner;
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
        combatComponent.OnDamaged += OnDamaged;
        combatComponent.OnDead += OnDead;
    }

    public void ResetMainBody()
    {
        combatComponent.ResetDead();
    }
    public void Spawn()
    {
        ResetMainBody();
        gameObject.SetActive(true);
    }

    private void OnDamaged()
    {
        _animator.SetTrigger("Hit");
    }
    private void OnDead()
    {
        OnFalseKnightDead.Invoke();
        gameObject.SetActive(false);
    }

    public override CombatComponent GetCombatComponent()
    {
        return combatComponent;
    }
}
