using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Windows;


public class EnemyBossKnightMoveComponent : MonoBehaviour
{
    public Direction dir = Direction.Right;

    private EnemyBossKnightController _controller;
    private Rigidbody2D _rigidbody;
    private Vector2 inputVector = Vector2.zero;
    public float nonInputDrag = 10f;
    public float inAirDrag = 1f;


    public float JumpPower = 20f;

    //충돌
    //디버깅용//Todo 인스펙터에만 보이도록
    //충돌검사
    [SerializeField]
    private int _collisionCounter = 0;

    public bool isGrounded = true;
    //지금은 공중이나 바닥이나 속도 같게
    //LaterDo:나중에 가능하면 부드럽게 속도를 제한하는법도 생각해보자 공중에서는 컨트롤이 먹먹히지게
    public float MaxSpeed = 5f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _controller = GetComponent<EnemyBossKnightController>();

        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_controller);
    }

    //Check Maxspeed And Move
    //If OverMaxSpeed Add Force?
    public void Move(Vector2 moveInput)
    {
        moveInput.y = 0f;
        Vector2 curVelocity = _rigidbody.velocity;
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
    //Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _collisionCounter++;
        isGrounded = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _collisionCounter--;
        if (_collisionCounter == 0)
        {
            isGrounded = false;
        }
    }

    public void StartJump()
    {
            _rigidbody.AddForce(JumpPower * Vector2.up, ForceMode2D.Impulse);
            isGrounded = false;
    }
    public void EndJump()
    {
        if(_rigidbody.velocity.y > 0f)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y*.5f);    
        }
    }
}