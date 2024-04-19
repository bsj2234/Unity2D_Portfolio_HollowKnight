using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToRandom : StateMachineBehaviour
{
    FalseKnight _fn;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _fn = animator.GetComponentInParent<FalseKnight>();
        _fn.JumpToFarPoint();
    }

}
