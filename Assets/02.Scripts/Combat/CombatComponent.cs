﻿
using System;
using UnityEngine;

[System.Serializable]
public class CombatComponent
{

    [field: SerializeField] public float mp { get; set; } = 0f;
    [field: SerializeField] public float maxMp { get; set; } = 100f;

    [field: SerializeField] public float DodgeInvincibleTime { get; set; } = 0.5f;
    [field: SerializeField] public bool isAttacking { get; set; } = false;
    [field: SerializeField] public Vector2 _attackDir { get; set; }
    [field: SerializeField] private float _attackingTime { get; set; } = 0f;
    [field: SerializeField] private float _stunTime { get; set; } = 0f;
    [field: SerializeField] private int continuableAttackCount { get; set; } = 0;


    [SerializeField] private float _maxHp = 100f;
    [SerializeField] private float _hp = 100f;
    [SerializeField] private bool _dead = false;

    [SerializeField] private float _invincibleTime = .1f;
    [SerializeField] private float _prevHitTime = 0f;
    public Transform _owner;
    private bool _defalutEffectOnDamaged;

    public Action OnDamaged { get; internal set; }
    public Action<CombatComponent> OnDamagedWAttacker { get; internal set; }
    public Action OnDead { get; internal set; }
    public Func<bool> AdditionalDamageCondition { get; internal set; }

    public GameObject[] additionalEffectOnHit;
    public float initalMaxHp;
    public bool noManaRegenOnHit = false;
    public Action OnHeal;

    public Vector3 prevAttackersPos { get; internal set; }

    public void Init(Transform owner, bool defaultEffectOnDamaged = true )
    {
        _owner = owner;
        _hp = _maxHp;
        initalMaxHp = _maxHp;
        _defalutEffectOnDamaged =defaultEffectOnDamaged;
    }

    public float GetHp() { return _hp; }
    public bool DealDamage(CombatComponent target, float damage)
    {
        bool isAttackSucceeded = target.TakeDamage(_owner.transform.position ,damage);
        if(isAttackSucceeded)
        {
            OnDamagedWAttacker?.Invoke(target);
            return false;
        }
        return true;
    }
    private bool IsDamageable()
    {
        if (Time.time < _prevHitTime + _invincibleTime)
        {
            return false;
        }
        if (_dead)
        {
            return false;
        }
        bool result = true;
        if (AdditionalDamageCondition != null)
        {
            result = result && AdditionalDamageCondition.Invoke();
        }
        if ( !result )
        {
            return false;
        }
        return true;
    }

    public void ResetDead()
    {
        _hp = _maxHp;
        _dead = false;
    }
    public void ResetHpWithRatio(float ratio)
    {
        _hp = _maxHp * ratio;
        _dead = false;
    }

    public bool IsDead()
    {
        return _dead;
    }
    public void SetMaxHp(float maxHp)
    {
        _maxHp = maxHp;
        ResetDead();
    }

    public bool TakeDamage(Vector3 position, float damage)
    {
        if (!IsDamageable())
            return false;
        CalcTakeDamage(damage);
        prevAttackersPos = position;
        if (_defalutEffectOnDamaged)
            PooledObjectSpawnManager.Instance.SpawnDefalutHitEffect(position, _owner.position);
        if (additionalEffectOnHit != null)
        {
            PooledObjectSpawnManager.Instance.SpawnBetween(additionalEffectOnHit, position, _owner.position, 1f, 3f);
        }
        return true;
    }

    public bool TakeDamage(float damage)
    {
        return TakeDamage(_owner.position, damage);
    }
    private void CalcTakeDamage(float damage)
    {
        _prevHitTime = Time.time;
        _hp -= damage;
        OnDamaged?.Invoke();
        if (_hp <= 0f)
        {
            _dead = true;
            OnDead?.Invoke();
        }
    }

    public void AddedMaxHp(float add)
    {
        _maxHp = initalMaxHp + add;
    }

    public float GetMaxHp()
    {
        return _maxHp;
    }

    internal void Heal(int v)
    {
        if(_hp < _maxHp)
        {
            _hp += v;
        }
        if( OnHeal != null )
        {
            OnHeal.Invoke();
        }
    }
    internal void Die()
    {
            TakeDamage(_owner.position , _hp);
    }
}