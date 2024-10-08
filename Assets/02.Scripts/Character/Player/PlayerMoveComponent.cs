
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
    public bool isMovable = true;
    //지금은 공중이나 바닥이나 속도 같게
    //LaterDo:나중에 가능하면 부드럽게 속도를 제한하는법도 생각해보자 공중에서는 컨트롤이 먹먹히지게
    public float MaxSpeed = 10f;
    public float Acceleration = 10f;
    public float _dashSpeed = 10f;
    private float _movableCoolDown = 0f;
    private float _DashTime = 0f;
    private bool dead = false;
    public bool IsMoveByAddForce;

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
        else
        {
            if(isMovable)
            {
                if(_controller.hasMoveInput)
                {
                    FlipToMoveInput();
                }
                else
                {
                    Look();
                }
            }
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
        if (GameManager.Instance.Player.GetCombatComponent().IsDead()) return;
        Vector2 curVelocity = _rigidbody.velocity;
        moveInput.y = 0f; //인풋 y 무시

        //대쉬시 무중력
        if (_DashTime > 0f)
        {
            float dashDir = (dir == Direction.Left ? -1f : 1f);
            _rigidbody.velocity = new Vector2(_dashSpeed * dashDir, 0f);
            return;
        }

        if (!IsMovable()) // 대쉬는 처리하고 Movable 체크
        {
            return;
        }
        if (IsMoveByAddForce)
            MoveByAddForce(moveInput, curVelocity);
        else
            MoveByVelocity(moveInput, curVelocity); 

    }


    private void MoveByAddForce(Vector2 moveInput, Vector2 curVelocity)
    {
        //입력이있으면 움직임 계산
        float inputDir = Mathf.Abs(moveInput.x) < .1f ? 0f : Mathf.Sign(moveInput.x);
        float velocityDir = Mathf.Sign(curVelocity.x);
        float velocityMag = Mathf.Abs(curVelocity.x);
        float desierdMag = MaxSpeed;
        float resultMag = desierdMag - velocityMag;
        Debug.Log(velocityMag);
        if (_controller.hasMoveInput)
        {

            //impulse 처럼 가속도가 빠르면 문제가 있다
            // 범위를 빠르게 제어하려면 정확한 값을 더하거나 빼 주어야 오차가 적다
            // 혹은 velocity로 값을 정확히 지정해줘야 한다.
            //입력과 진행 방향이 같으면
            //최대속도를 넘었을 경우에는 아무것도안함
            //방향이 다르면 그냥 뒤돔
            if (inputDir == velocityDir)
            {
                Debug.Log("Same");
                if (velocityMag > MaxSpeed + .1f)
                {
                    Debug.Log("OverSpeed");
                    //속도 초과시 현재속도 - 최대속도의 힘 만큼 으로 속도 제한
                    float accel = Mathf.Min(velocityMag - MaxSpeed, Acceleration);
                    _rigidbody.AddForce(new Vector2(-velocityDir * accel, 0f), ForceMode2D.Impulse);
                }
                else
                {
                    Debug.Log("UnderSpeed");
                    float accel = Mathf.Min(MaxSpeed - velocityMag, Acceleration);
                    //최대속도 - 현재속도 만큼의 힘으로 이동
                    _rigidbody.AddForce(new Vector2(velocityDir * accel, 0f), ForceMode2D.Impulse);
                }
            }
            else
            {
                Debug.Log(message: "Diff");
                // 최대속보다 작을때만 뒤돌시 현 벨로시티의 방향을 바꿈
                float accel = Mathf.Max(velocityMag, Acceleration);
                if (velocityMag < MaxSpeed)
                    _rigidbody.AddForce(new Vector2(-velocityDir * accel, 0f), ForceMode2D.Impulse);
                else
                    _rigidbody.AddForce(new Vector2(Acceleration * -velocityDir, 0f), ForceMode2D.Impulse);

            }
        }
        //입력이 없으면
        //최대속보다빠르면 가능한속도씩빼다가
        //최대속보다 작아지면
        //아니면 현재속도만큼뺌
        //충분히 적으면 정지
        else
        {
            //최대속보다 크면 감속시키고
            if (velocityMag > MaxSpeed + .1f)
            {
                Debug.Log("OverMaxSpeed");
                _rigidbody.AddForce(new Vector2(nonInputDrag * -velocityDir, 0f), ForceMode2D.Impulse);
            }
            //작으면 
            else
            {
                Debug.Log("UnderMaxSpeed");
                //멈춰
                if (velocityMag >= .1f)
                {
                    Debug.Log("OverDeadZone");
                    _rigidbody.AddForce(new Vector2(-curVelocity.x, 0f), ForceMode2D.Impulse);
                }
                else
                {
                    Debug.Log("UnderDeadZone");
                    _rigidbody.velocity = new Vector2(0f, curVelocity.y);
                }
            }
        }
    }
    private void MoveByVelocity(Vector2 moveInput, Vector2 curVelocity)
    {
        _rigidbody.velocity = new Vector2(moveInput.x * MaxSpeed, curVelocity.y);
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
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
        }
        _rigidbody.AddForce(knockBackDir * knockbackSpeed * knockbackRatio, ForceMode2D.Impulse);

    }
    public void KnockBack(Vector2 knockBackDir, float normalizedKnockbackPower, float attackSpeed)
    {

        //조금더 높게 공속과 관련이 있게
        //최대 높이를 지정해주고 싶다
        //전에 밟았던 땅을 기주으로 할 수도있고
        //현재 공격중인 오브젝트의 높이를 기준으로 할 수도 있다
        //아니면 높이 / 공속 하면 높이가 자연히 제한되겠지 5의 높이를 찍고싶으면 9.81 하면 1초간 올라가다 떨어질거고 속도는 점점 내려갈거야 평균 4.905니까 속도가 그럼 거리가 그만큼 나오겠네 
        // 만약 500 넣으면 500~ 0까지 떨어질건데 초당 9.81 씩 떨어져 그냥 초당 10씩이라고 가정하면 50 초가 지나면 0인데 50 * 500/2 겠는데 
        // 그럼 공속으로 거리를 나누고싶어
        // 거리가 2점프되면 좋겠어 속도는 아아 되겠지
        // 

        if (normalizedKnockbackPower <= 0f)
        {
            return;
        }
            float knockbackRatio = 50f;
        if (knockBackDir == Vector2.up)
        {
            knockbackRatio = 1f;
            float gravity = 9.81f * _rigidbody.gravityScale;
            float airbontime = gravity * attackSpeed;
            _rigidbody.velocity = new Vector2( _rigidbody.velocity.x , 0f);
            _rigidbody.AddForce(knockBackDir * knockbackRatio * airbontime * normalizedKnockbackPower, ForceMode2D.Impulse);
        }
        else
        {
            _rigidbody.AddForce(knockBackDir * knockbackRatio * normalizedKnockbackPower, ForceMode2D.Impulse);
        }

    }
    private void BlockMoveForSeconds(float time)
    {
        _movableCoolDown = time;
    }
    public virtual void Flip()
    {
        Quaternion right = Quaternion.Euler(0f, 0f, 0f);
        Quaternion left = Quaternion.Euler(0f, 180f, 0f);
        float curAngle = transform.rotation.eulerAngles.y;

        if (curAngle == 180f)
        {
            transform.rotation = right;
            dir = Direction.Right;
        }
        else
        {
            transform.rotation = left;
            dir = Direction.Left;
        }
    }
    public void SetPlayerDirection(Direction desiredDir)
    {
        Quaternion right = Quaternion.Euler(0f, 0f, 0f);
        Quaternion left = Quaternion.Euler(0f, 180f, 0f);
        float curAngle = transform.rotation.eulerAngles.y;

        if (dir == desiredDir)
            return;

        if (desiredDir == Direction.Right)
        {
            transform.rotation = right;
            dir = Direction.Right;
        }
        else
        {
            transform.rotation = left;
            dir = Direction.Left;
        }
    }
    public void Look()
    {
        if (_controller.GetLookDir().x <= 0f)
        {
            SetPlayerDirection(Direction.Left);
        }
        else
        {
            SetPlayerDirection(Direction.Right);
        }
    }
    public void FlipToMoveInput()
    {
        if (_controller.moveDirInput.x < 0f)
        {
            SetPlayerDirection(Direction.Left);
        }
        else if (_controller.moveDirInput.x > 0f)
        {
            SetPlayerDirection(Direction.Right);
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


}
