using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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
    private Vector2 inputVector = Vector2.zero;
    public float nonInputDrag = 10f;
    public float inAirDrag = 1f;

    private float initailGravity = 0f;

    public float JumpPower = 20f;

    public bool isGrounded = true;
    //지금은 공중이나 바닥이나 속도 같게
    //LaterDo:나중에 가능하면 부드럽게 속도를 제한하는법도 생각해보자 공중에서는 컨트롤이 먹먹히지게
    public float MaxSpeed = 5f;
    public float _dashSpeed = 10f;
    private bool _dashing = false;
    private float _movableCoolDown = 0f;
    private float _zeroGravityTime = 0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _controller = GetComponent<PlayerController>();

        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_controller);

        initailGravity = _rigidbody.gravityScale;
    }

    private void Update()
    {
        //set cooldown
        if(_movableCoolDown >= 0f)
        {
            _movableCoolDown -= Time.deltaTime;
            _zeroGravityTime -= Time.deltaTime;
        }
    }

    //넉백 어쩔건데
    //맞으면 뒤로 날라가야지
    //근데지금 매프레임마다 입력값에 움직임을 종속시킴
    //맞을때문 풀어?
    public void MovementUpdate(Vector2 moveInput)
    {
        Vector2 curVelocity = _rigidbody.velocity;

        //대쉬시 무중력
        if (_zeroGravityTime > 0f)
        {
            _rigidbody.gravityScale = 0f;
            _rigidbody.velocity = new Vector2(curVelocity.x, 0f);
        }
        else
        {
            _rigidbody.gravityScale = initailGravity;
        }
        if(!IsMovable())
        {
            return;
        }
        moveInput.y = 0f;
        if (_controller.hasMoveInput)
        {
            _rigidbody.velocity = new Vector2(moveInput.x * MaxSpeed, _rigidbody.velocity.y);
            dir = (moveInput.x < 0f) ? Direction.Left : Direction.Right;
        }
        else
        {
            _rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
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
        if(isGrounded == true)
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
        _movableCoolDown = .5f;
        _zeroGravityTime = .5f;
        _rigidbody.velocity = (dir == Direction.Left) ?
            Vector2.left * _dashSpeed : Vector2.right * _dashSpeed;
        
    }

    public void KnockBack(Vector2 knockBackDir, float knockbackSpeed)
    {
        _movableCoolDown = .25f;
        //StartCoroutine(BlockMoveForSeconds())
        _rigidbody.velocity = knockBackDir * knockbackSpeed;
    }
    public void KnockBackUp(Direction knockBackDir, float knockbackSpeed)
    {
        //StartCoroutine(BlockMoveForSeconds())
        _rigidbody.AddForce(Vector2.up * knockbackSpeed);
    }

    private void BlockMoveForSeconds(float time)
    {
        _movableCoolDown = time;
    }
}
public static class VectorExtender
{
    public static bool IsOpposite(this Vector2 v1, Vector2 v2)
    {
        return (Vector2.Dot(v1, v2) < 0f) ? true : false;
    }
}
