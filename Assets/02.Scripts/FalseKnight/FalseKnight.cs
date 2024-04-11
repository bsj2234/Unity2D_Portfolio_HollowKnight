
using UnityEngine;
using UnityEngine.Assertions;

public enum FalseKnightState
{
    Idle, JumpAttack, Jump, GroundAttack, Rampage, Stun, Dead
}
public class FalseKnight : MonoBehaviour
{

    public FalseKnightState state;
    private Animator _animator;
    private Transform target;
    private Rigidbody2D _rigidbody;
    public bool isGrounded;
    private float groundIgnoreTime = 0f;

    private void Awake()
    {
        _animator = transform.GetComponentInChildren<Animator>();
        Assert.IsNotNull( _animator );
        _rigidbody = GetComponent<Rigidbody2D>();
        target = GameManager.Instance.GetPlayer().transform;
    }

    public void ChangeState(FalseKnightState newState)
    {
        state = newState;
        switch (newState)
        {
            case FalseKnightState.Idle:
                _animator.SetTrigger("Idle");
                break;
            case FalseKnightState.JumpAttack:
                _animator.SetTrigger("JumpAttack");
                break;
            case FalseKnightState.Jump:
                _animator.SetTrigger("Jump");
                break;
            case FalseKnightState.GroundAttack:
                _animator.SetTrigger("GroundAttack");
                break;
            case FalseKnightState.Rampage:
                _animator.SetTrigger("Rampage");
                break;
            case FalseKnightState.Stun:
                _animator.SetTrigger("Stun");
                break;
            case FalseKnightState.Dead:
                _animator.SetTrigger("Dead");
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if(groundIgnoreTime > 0f)
        {
            groundIgnoreTime -= Time.deltaTime;
            _animator.SetBool("AlmostOnGround", false);
        }
        else
        {
            if (isGrounded)
                _animator.SetBool("AlmostOnGround", true);
            else
                _animator.SetBool("AlmostOnGround", false);
        }

    }

    public void JumpToTarget(float time)
    {
        isGrounded = false;
        groundIgnoreTime = .2f;
        float distance = target.position.x - transform.position.x;
        _rigidbody.AddForce(new Vector2(distance / time, 9.81f * time * .5f), ForceMode2D.Impulse);
        FocusToPlayer();
    }
    public void FocusToPlayer()
    {
        if (target.position.x - transform.position.x < 0f)
        {
            transform.rotation = Quaternion.Euler(0f,180f,0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    public void JumpToRandom(float time)
    {
        float distance = Random.Range(-20f, 20f);
        _rigidbody.AddForce(new Vector2(distance / time, 9.81f * time * .5f), ForceMode2D.Impulse);
        FocusToPlayer();
    }


    private void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            isGrounded |= normal.y >= .9f;
        }
    }

    //Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        if (groundIgnoreTime > 0)
            isGrounded = false;
        if (isGrounded)
            Debug.Log(collision.gameObject.name);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        if (groundIgnoreTime > 0)
            isGrounded = false;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

}
