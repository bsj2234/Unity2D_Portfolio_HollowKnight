
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
    //������ �����̳� �ٴ��̳� �ӵ� ����
    //LaterDo:���߿� �����ϸ� �ε巴�� �ӵ��� �����ϴ¹��� �����غ��� ���߿����� ��Ʈ���� �Ը�������
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

    //�˹� ��¿�ǵ�
    //������ �ڷ� ���󰡾���
    //�ٵ����� �������Ӹ��� �Է°��� �������� ���ӽ�Ŵ
    //������ Ǯ��? ������ ���� ����
    //������ ���� �ణ�� �־��
    //add ���޽��� ����
    //���� �����϶� �ִ�ӵ����� ũ�� ���� �ʱ�
    //�ִ�ӵ����� ����ӵ��� �A ���� �ֱ�
    //�ִ���� �� ������ ���̳ʽ��ε����ֱ� �ؾ��ϴµ�
    public void MovementUpdate(Vector2 moveInput)
    {
        if(GameManager.Instance.Player.isDead) return;
        Vector2 curVelocity = _rigidbody.velocity;

        //�뽬�� ���߷�
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
        //�Է���������

        if (_controller.hasMoveInput)
        {
            float inputDir = Mathf.Sign(moveInput.x);
            float velocityDir = Mathf.Sign(curVelocity.x);
            float velocityMag = Mathf.Abs(curVelocity.x);
            float desierdMag = MaxSpeed;
            float resultMag = desierdMag - velocityMag;

            //�Է������� ������ ��
            if (inputDir == -1) dir = Direction.Left;
            else if (inputDir == 1) dir = Direction.Right;

            //�Է°� ���� ������ ������
            //�ִ�ӵ��� �Ѿ��� ��쿡�� �ƹ��͵�����
            //������ �ٸ��� �׳� �ڵ�
            //�Է��� ������


            //impulse ó�� ���ӵ��� ������ ������ �ִ�
            // ������ ������ �����Ϸ��� ��Ȯ�� ���� ����� �� �ְ� ���ϰų� �� �ְ�
            //velocity�� ���� ��Ȯ�� ��������� �Ѵ�.
            //velocity�� ���� �����ϸ� ���� �ִ� 
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
        //�Է��� ������
        //�ִ�Ӻ��ٺ����� �����Ѽӵ������ٰ�
        //�ִ�Ӻ��� �۾�����
        //�ƴϸ� ����ӵ���ŭ��
        //����� ������ ����
        else
        {
            float velocityX = _rigidbody.velocity.x;
            float velocityMag = Mathf.Abs(_rigidbody.velocity.x);
            float DirX = Mathf.Sign(velocityX);
            //�ִ�Ӻ��� ũ�� ���ӽ�Ű��
            if (velocityMag > MaxSpeed + .1f)
            {
                _rigidbody.AddForce(new Vector2(MaxSpeed * -DirX, 0f), ForceMode2D.Impulse);
            }
            //������ 
            else
            {
                //����
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
