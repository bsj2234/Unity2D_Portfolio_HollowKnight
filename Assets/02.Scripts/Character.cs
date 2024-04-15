using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IFightable
{
    void IFightable.DealDamage(IFightable target, float damage)
    {
        throw new NotImplementedException();
    }

    void IFightable.DealFixedDamage(IFightable target, float damage)
    {
        throw new NotImplementedException();
    }

    float IFightable.GetHp()
    {
        throw new NotImplementedException();
    }

    void IFightable.TakeDamage(float damage, Vector2 Attackerpos)
    {
        throw new NotImplementedException();
    }
}
