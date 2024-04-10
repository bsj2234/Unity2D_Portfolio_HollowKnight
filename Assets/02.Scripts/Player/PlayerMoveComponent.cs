using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public enum Direction { Left, Right }
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


    public float JumpPower = 20f;

    //�浹
    //������//Todo �ν����Ϳ��� ���̵���
    //�浹�˻�
    [SerializeField]
    private int _collisionCounter = 0;

    public bool isGrounded = true;
    //������ �����̳� �ٴ��̳� �ӵ� ����
    //LaterDo:���߿� �����ϸ� �ε巴�� �ӵ��� �����ϴ¹��� �����غ��� ���߿����� ��Ʈ���� �Ը�������
    public float MaxSpeed = 5f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _controller = GetComponent<PlayerController>();

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
public static class VectorExtender
{
    public static bool IsOpposite(this Vector2 v1, Vector2 v2)
    {
        return (Vector2.Dot(v1, v2) < 0f) ? true : false;
    }
}