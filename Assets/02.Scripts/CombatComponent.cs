
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
    private Vector3 _prevAttackerPos;
    private Transform _owner;
    private bool _defalutEffectOnDamaged;

    public Action OnDamaged { get; internal set; }
    public Action OnDead { get; internal set; }


    public void Init(Transform owner, bool defaultEffectOnDamaged = true )
    {
        _owner = owner;
        _hp = _maxHp;
        _defalutEffectOnDamaged=defaultEffectOnDamaged;
    }

    public float GetHp() { return _hp; }
    public void DealDamage(CombatComponent target, float damage)
    {
        target.TakeDamage(damage);
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
        _prevHitTime = Time.time;
        return true;
    }

    public void ResetHp()
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
        ResetHp();
    }

    public void TakeDamage(Vector3 position, float damage)
    {
        if (!IsDamageable())
            return;
        TakeDamage(damage);
        if(_defalutEffectOnDamaged)
            ObjectSpawnManager.Instance.SpawnDefalutHitEffect(position, _owner.position);
    }
    private void TakeDamage(float damage)
    {
        _hp -= damage;
        OnDamaged?.Invoke();
        if (_hp <= 0f)
        {
            _dead = true;
            OnDead?.Invoke();
        }
    }
}