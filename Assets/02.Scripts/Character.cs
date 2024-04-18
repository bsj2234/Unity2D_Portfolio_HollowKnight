using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IFightable
{
    public virtual void DealDamage(IFightable target, float damage)
    {
        throw new NotImplementedException();
    }

    public virtual void DealFixedDamage(IFightable target, float damage)
    {
        throw new NotImplementedException();
    }

    public virtual float GetHp()
    {
        throw new NotImplementedException();
    }

    public virtual void TakeDamage(float damage, Vector2 Attackerpos)
    {
        throw new NotImplementedException();
    }

    public virtual bool IsDead()
    {
        throw new NotImplementedException ();
    }
}
