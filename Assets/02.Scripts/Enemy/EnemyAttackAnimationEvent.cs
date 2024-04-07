using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyAttackAnimationEvent : MonoBehaviour
{
    private Dictionary<string, BoxCollider2D> _animationCollider = new();
    private EnemyBossKnight _player;
    private Animator _animator;

    private void Awake()
    {
        _player = GetComponentInParent<EnemyBossKnight>();
        _animator = GetComponent<Animator>();
        Assert.IsNotNull(_player);
        Assert.IsNotNull( _animator );
    }
    public void AttackContinueCheck()
    {
        if (!_player.isPendingAttack)
        {
            _player.isAttacking = false;
            _animator.SetBool("Anim_IsAttacking_Slash", false);
        }
        _player.isPendingAttack = false;
    }
    public void AttackCollisionCheck()
    {
    }
}
