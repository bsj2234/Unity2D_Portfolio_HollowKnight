
using System;
using UnityEngine;

[System.Serializable]
public class CombatComponent
{
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
        TakeDamage(damage);
        prevAttackersPos = position;
        if (_defalutEffectOnDamaged)
            ObjectSpawnManager.Instance.SpawnDefalutHitEffect(position, _owner.position);
        if(additionalEffectOnHit != null)
        {
            ObjectSpawnManager.Instance.SpawnBetween(additionalEffectOnHit, position, _owner.position, 1f, 3f);
        }
        return true;
    }
    private void TakeDamage(float damage)
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
}