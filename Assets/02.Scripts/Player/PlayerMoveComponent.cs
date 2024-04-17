
using System;
using UnityEngine;
using UnityEngine.Assertions;

public enum Direction { Left, Right ,Up, Down}
public static class DirctionEval
{
    public static float Get1D(this Direction dir)
    {
        return (dir == Direction.Left) ? -1f : 1f;
    }
    public static Vector2 Get2D(this Direction dir)
    {
        return (dir == Direction.Left) ? new Vector2(-1f, 0f) : new Vector2(1f, 0f);
    }
}

public class PlayerMoveComponent : MonoBehaviour
{
    public Direction dir = Direction.Right;

    private PlayerController _controller;
    private Rigidbody2D _rigidbody;
    public float nonInputDrag = 10f;
    public float inAirDrag = 1f;

    public float JumpPower = 20f;

    public bool isGrounded = true;
    //지금은 공중이나 바닥이나 속도 같게
    //LaterDo:나중에 가능하면 부드럽게 속도를 제한하는법도 생각해보자 공중에서는 컨트롤이 먹먹히지게
    public float MaxSpeed = 5f;
    public float _dashSpeed = 10f;
    private float _movableCoolDown = 0f;
    private float _DashTime = 0f;
    private bool dead = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _controller = GetComponent<PlayerController>();

        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_controller);
    }

    private void Update()
    {
        if(dead)
            return;
        //set cooldown
        if(_movableCoolDown >= 0f)
        {
            _movableCoolDown -= Time.deltaTime;
            _DashTime -= Time.deltaTime;
        }
    }

    //넉백 어쩔건데
    //맞으면 뒤로 날라가야지
    //근데지금 매프레임마다 입력값에 움직임을 종속시킴
    //맞을때 풀어? 움직일 수가 없어
    //경직을 아주 약간만 넣어볼까
    //add 임펄스로 넣자
    //같은 방향일때 최대속도보다 크면 넣지 않기
    //최대속도에서 현재속도를 뺸 값만 넣기
    //최대속이 더 빠르면 마이너스로돌려주긴 해야하는데
    public void MovementUpdate(Vector2 moveInput)
    {
        if(GameManager.Instance.Player.isDead) return;
        Vector2 curVelocity = _rigidbody.velocity;

        //대쉬시 무중력
        if (_DashTime > 0f)
        {
            float dashDir = (dir == Direction.Left ? -1f : 1f);
            _rigidbody.velocity = new Vector2(MaxSpeed * 2f * dashDir, 0f);
        }
        if(!IsMovable())
        {
            return;
        }
        moveInput.y = 0f;
        //입력이있으면

        if (_controller.hasMoveInput)
        {
            float inputDir = Mathf.Sign(moveInput.x);
            float velocityDir = Mathf.Sign(curVelocity.x);
            float velocityMag = Mathf.Abs(curVelocity.x);
            float desierdMag = MaxSpeed;
            float resultMag = desierdMag - velocityMag;

            //입력있으면 그쪽을 봄
            if (inputDir == -1) dir = Direction.Left;
            else if (inputDir == 1) dir = Direction.Right;

            //입력과 진행 방향이 같으면
            //최대속도를 넘었을 경우에는 아무것도안함
            //방향이 다르면 그냥 뒤돔
            //입력이 없으면


            //impulse 처럼 가속도가 빠르면 문제가 있다
            // 범위를 빠르게 제어하려면 정확한 값을 계산할 수 있게 더하거나 뺴 주고
            //velocity로 값을 정확히 지정해줘야 한다.
            //velocity를 직접 지정하면 전에 있던 
            if (inputDir == velocityDir)
            {
                if(velocityMag > MaxSpeed + .1f)
                {
                    _rigidbody.AddForce(new Vector2(-velocityDir * (velocityMag - MaxSpeed), 0f), ForceMode2D.Impulse);


                }
                else
                {
                    _rigidbody.AddForce(new Vector2(velocityDir * (MaxSpeed - velocityMag), 0f), ForceMode2D.Impulse);
                }
            }
            else
            {
                _rigidbody.AddForce(new Vector2(-velocityDir * MaxSpeed, 0f), ForceMode2D.Impulse);
            }
        }
        //입력이 없으면
        //최대속보다빠르면 가능한속도씩빼다가
        //최대속보다 작아지면
        //아니면 현재속도만큼뺌
        //충분히 적으면 정지
        else
        {
            float velocityX = _rigidbody.velocity.x;
            float velocityMag = Mathf.Abs(_rigidbody.velocity.x);
            float DirX = Mathf.Sign(velocityX);
            //최대속보다 크면 감속시키고
            if (velocityMag > MaxSpeed + .1f)
            {
                _rigidbody.AddForce(new Vector2(MaxSpeed * -DirX, 0f), ForceMode2D.Impulse);
            }
            //작으면 
            else
            {
                //멈춰
                if(velocityX >= .1f)
                {
                    _rigidbody.AddForce(new Vector2(-velocityX, 0f), ForceMode2D.Impulse);
                }
                else
                {
                    _rigidbody.velocity = new Vector2(0f , curVelocity.y);
                }
            }
        }
    }

    private bool IsMovable()
    {
        return _movableCoolDown <= 0f;
    }

    private void EvaluateCollision(Collision2D collision)
    {
        for(int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            isGrounded |= normal.y >= .9f;
        }
    }

    //Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    public void StartJump()
    {
        if (dead) return;
        if (isGrounded == true)
        {
            _rigidbody.AddForce(JumpPower * Vector2.up, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }
    public void EndJump()
    {
        if(_rigidbody.velocity.y > 0f)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y*.5f);    
        }
    }

    public void Dash()
    {
        if(dead) return;
        _movableCoolDown = .25f;
        _DashTime = .25f;
        //_rigidbody.velocity = (dir == Direction.Left) ? Vector2.left * _dashSpeed : Vector2.right * _dashSpeed;
        
    }

    public void KnockBack(Vector2 knockBackDir, float knockbackSpeed)
    {
        float knockbackRatio = 2f;
        if (knockBackDir == Vector2.up)
        {
            knockbackRatio = 1f;
        }
        _rigidbody.AddForce(knockBackDir * knockbackSpeed * knockbackRatio,ForceMode2D.Impulse);
    }

    private void BlockMoveForSeconds(float time)
    {
        _movableCoolDown = time;
    }

    public virtual void Flip()
    {
        Quaternion right = Quaternion.Euler(0f, 180f, 0f);
        Quaternion left = Quaternion.Euler(0f, 0f, 0f);
        float curAngle = transform.rotation.eulerAngles.y;

        if (curAngle == 180f)
        {
            transform.rotation = left;
        }
        else
        {
            transform.rotation = right;
        }
    }

    public void Dead()
    {
        _rigidbody.velocity = Vector2.zero;
        dead = true;
    }

    public void ResetMove()
    {
        dead = false;
        isGrounded = true;
        _movableCoolDown = 0f;
        _DashTime = 0f;
}

    public void OnDamaged()
    {
        _rigidbody.velocity = Vector2.zero;
    }
}
