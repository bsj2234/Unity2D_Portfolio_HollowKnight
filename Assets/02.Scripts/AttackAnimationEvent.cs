using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class AttackAnimationEvent : MonoBehaviour
{
    private Dictionary<string, BoxCollider2D> _animationCollider = new();
    private PlayerController _controller;
    private Animator _animator;

    private void Awake()
    {
        _controller = GetComponentInParent<PlayerController>();
        _animator = GetComponent<Animator>();
        Assert.IsNotNull(_controller);
        Assert.IsNotNull( _animator );
    }
    public void AttackContinueCheck()
    {
        if (!_controller.isPendingAttack)
        {
            _controller.isAttacking = false;
            _animator.SetBool("Anim_IsAttacking_Slash", false);
        }
        _controller.isPendingAttack = false;
    }
    public void AttackCollisionCheck()
    {
    }
}
