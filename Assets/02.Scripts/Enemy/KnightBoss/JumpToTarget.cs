using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class JumpToTarget : StateMachineBehaviour
{
    private EnemyBossKnight _enemy;
    public float TimeToTarget;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_enemy == null)
        {
            _enemy = animator.transform.GetComponentInParent<EnemyBossKnight>();
            Assert.IsNotNull(_enemy);
        }
        _enemy.JumpToNearWallSide(TimeToTarget);
    }
}
