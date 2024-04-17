using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Player _player;
    private Animator _animator;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
        Assert.IsNotNull(_player);
        Assert.IsNotNull( _animator );
    }
    public void OnDead()
    {
        _player.OnDeadAnimEvent();
    }
}
