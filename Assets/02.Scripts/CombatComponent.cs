
using UnityEngine;

[System.Serializable]
public class CombatComponent
{
    [SerializeField] private float _initialHp = 100f;
    [SerializeField] private float _hp = 100f;
    [SerializeField] private bool _dead = false;
    public float GetHp() { return _hp; }
    public void DealDamage(CombatComponent target, float damage)
    {
        target.TakeDamage(damage);
    }
    public void TakeDamage(float damage)
    {
        if (_dead)
        {
            return;
        }

        _hp -= damage;

        if (_hp <= 0f)
        {
            _dead = true;
        }
    }


    public void ResetHp()
    {
        _hp = _initialHp;
        _dead = false;
    }
    public void ResetHpWithRatio(float ratio)
    {
        _hp = _initialHp * ratio;
        _dead = false;
    }

    public bool IsDead()
    {
        return _dead;
    }
}