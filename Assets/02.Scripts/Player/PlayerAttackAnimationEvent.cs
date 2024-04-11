using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerAttackAnimationEvent : MonoBehaviour
{
    private Dictionary<string, BoxCollider2D> _animationCollider = new();
    private Player _player;
    private Animator _animator;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
        Assert.IsNotNull(_player);
        Assert.IsNotNull( _animator );
    }
    public void AttackContinueCheck()
    {
        //���� ������ ������� ������
        //���ݻ��� ����
        if (!_player.isPendingAttack)
        {
            _player.isAttacking = false;
            _animator.SetBool("Anim_IsAttacking_Slash", false);
        }
        _player.isPendingAttack = false;
    }
}
