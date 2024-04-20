using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FocusToPlayer : StateMachineBehaviour
{

    FalseKnight _fn;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_fn == null)
        {
            _fn = animator.GetComponentInParent<FalseKnight>();
        }
        _fn.FocusToPlayer();
    }
}
