using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttack : StateMachineBehaviour
{
    Rigidbody2D _rb;
    FalseKnight _fn;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rb = animator.transform.GetComponentInParent<Rigidbody2D>();
        _rb.velocity = Vector3.zero;
        _fn = animator.GetComponentInParent<FalseKnight>();
        _fn.JumpToTarget(2f);
    }
}
