using UnityEngine;

public class FalseKnightSelectPattern : StateMachineBehaviour
{
    public float CoolDown = .5f;
    Timer _timer = new Timer();
    public FalseKnightState debugAction = FalseKnightState.Idle;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer.SetTimer(CoolDown);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_timer.IsTimeOver())
        {
            _timer.ResetTime();
            FalseKnightState start = (FalseKnightState)Random.Range(0, 6);
            if (debugAction != FalseKnightState.Idle)
                start = debugAction;
            switch (start)
            {
                case FalseKnightState.Idle:
                    animator.SetTrigger("Idle");
                    break;
                case FalseKnightState.JumpAttack:
                    animator.SetTrigger("JumpAttack");
                    break;
                case FalseKnightState.Jump:
                    animator.SetTrigger("Jump");
                    break;
                case FalseKnightState.GroundAttack:
                    animator.SetTrigger("GroundAttack");
                    break;
                case FalseKnightState.Rampage:
                    animator.SetTrigger("Rampage");
                    break;
                default:
                    break;
            }
        }
    }
}
