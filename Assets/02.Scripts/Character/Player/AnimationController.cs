using Siko;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerCombatStates
{
    Running,
    Stunned,
    AttackingUp,
    AttackingNormal,
    AttackingNormalAlt,
    AttackingDown,

}
public class AnimationController:MonoBehaviour
{
    //player state is
    public States<PlayerCombatStates> PlayerCombatState { get; private set; }

    private Animator _animator;

    private void Awake()
    {
        PlayerCombatState.OnStateEnabled += => { _animator.SetBool("Stunned", true); };

        //state 우선순위
        _combatComponent.OnDamaged += (float delay) => {
            PlayerCombatState.StateForSecond(PlayerCombatStates.Stunned, delay);
            };
        _moveComponent.OnNotMovable += => _animator.SetTrigge
    }

}

namespace Siko
{
    public class States<T> where T : Enum
    {

        private State[] _states = new State[Enum.GetValues(typeof(T)).Length];

        public bool GetState(T state)
        {
            int stateInt = (int)(object)state;
            if (_stateDelay[stateInt] < -0f)
            {
                return false;
            }
            return (_stateTimer[stateInt] <= _stateTimer[stateInt] + _stateDelay[stateInt]) ? true : false;
        }

        public void SetState(T state, bool condition)
        {
            int stateInt = (int)(object)state;
            _stateTimer[stateInt] = condition ? -1f : 0f;
        }
        //cache current time and delay
        public void StateForSecond(T state, float delay)
        {
            int stateInt = (int)(object)state;
            _stateTimer[stateInt] = Time.time;
            _stateDelay[stateInt] = delay;
        }

    }
}

public class State
{
    private bool _isEnabled = false;
    public bool IsEnabled { get { return _isEnabled; } 
        set 
        { 
            //전과 다른 값으로 set되면
            if(value == _isEnabled)
            {
                return;
            }
            if (value) OnEnabled?.Invoke();
            else OnDisabled?.Invoke();
            _isEnabled = value;
        }
    }

    public System.Action OnEnabled { get; set; }
    public System.Action OnDisabled { get; set; }

    private float _delay = 0f;
    private float _startTime = 0f;
    public void SetStateWithTime(bool condition, float delay)
    {
        _startTime = Time.time;
        _delay = delay;
        
    }
}