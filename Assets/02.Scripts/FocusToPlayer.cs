using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FocusToPlayer : StateMachineBehaviour
{
    private EnemyBossKnight _enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_enemy == null)
        {
            _enemy = animator.transform.GetComponentInParent<EnemyBossKnight>();
            Assert.IsNotNull(_enemy);
        }
        _enemy.FocusToPlayer();
    }
}
