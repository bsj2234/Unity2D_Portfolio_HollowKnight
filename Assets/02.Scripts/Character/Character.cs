using UnityEngine;

public abstract class Character : MonoBehaviour, IFightable
{
    public abstract CombatComponent GetCombatComponent();
}
