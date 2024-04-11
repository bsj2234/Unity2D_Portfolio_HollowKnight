using UnityEngine;

public class FalseKnightIdle : StateMachineBehaviour
{
    public float CoolDown = .5f;
    float Timer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Timer = CoolDown;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Timer -= Time.deltaTime;
        if(Timer <= 0)
        {
            Timer = CoolDown;
            FalseKnightState start = (FalseKnightState)Random.Range(0, 6);
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
                case FalseKnightState.Stun:
                    animator.SetTrigger("Stun");
                    break;
                case FalseKnightState.Dead:
                    animator.SetTrigger("Dead");
                    break;
                default:
                    break;
            }
        }
    }
}
