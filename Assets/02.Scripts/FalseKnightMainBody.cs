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

    public float _invincibleTime = 0f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Assert.IsNotNull(_animator);
        Assert.IsNotNull(combatComponent);
        combatComponent.SetMaxHp(70);
    }

    private void Update()
    {
        if(_invincibleTime > 0f) { _invincibleTime -= Time.deltaTime; }
        else { _invincibleTime = 0f;}
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
        if(_invincibleTime > 0f)
            return;
        combatComponent.TakeDamage(attackerPos, damage);
        _animator.SetTrigger("Hit");
        _invincibleTime = .1f;
        ObjectSpawnManager.Instance.SpawnBetween(damagedEffects, attackerPos, transform.position,1f , 4f);
        if (combatComponent.IsDead())
        {
            if (OnDead != null)
            {
                OnDead.Invoke();
            }
            gameObject.SetActive(false);
        }
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
}
