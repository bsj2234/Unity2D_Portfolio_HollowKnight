using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Windows;


public class CrawlidMoveComponent : MonoBehaviour
{
    public Direction dir = Direction.Left;
    public float rayOffset;
    public float rayDistance;
    public float hp = 100;
    public LayerMask ground = 1 << 7;

    public bool isGrounded = true;
    public float MaxSpeed = 5f;

    private Transform _sprite;

    private Animator _animator;
    private void Awake()
    {
        _sprite = transform.GetChild(0);
        Assert.IsNotNull(_sprite);
        _animator = _sprite.GetComponent<Animator>();
        Assert.IsNotNull(_animator);
    }

    private void FixedUpdate()
    {
        Move(Vector2.up);
    }
    //다른방법 찾자
    public void Move(Vector2 moveInput)
    {
        //check edge
        RaycastHit2D ground_hit = Physics2D.Raycast(transform.position + (-transform.right) * rayOffset, -transform.up, rayDistance, ground);
        RaycastHit2D front_hit = Physics2D.Raycast(transform.position , -transform.right, rayDistance, ground);
        Debug.DrawRay(transform.position + (-transform.right) * rayOffset, -transform.up * rayDistance,Color.red,0.01f);

        //rotaton edge
        if(ground_hit.collider == null)
        {
            if(dir == Direction.Left)
            {
                transform.Rotate(0f, 0f, 90f);
                _sprite.Rotate(0f, 0f, -90f);
                transform.Translate(-.1f, 0f, 0f);
            }
            else
            {
                transform.Rotate(0f, 0f, -90f);
            }
        }
        //rotate on wall
        if(front_hit.collider != null)
        {
            if (dir == Direction.Left)
            {
                transform.Rotate(0f, 0f, -90f);
                _sprite.Rotate(0f, 0f, 90f);
                transform.Translate(-.1f, 0f, 0f);
            }
            else
            {
                transform.Rotate(0f, 0f, 90f);
            }
        }
        //move
        transform.Translate(-.5f * Time.fixedDeltaTime, 0f, 0f);
        //slow rotate
        //0 ~ 5
        float centered_z = Mathf.Abs(_sprite.localEulerAngles.z - 180f) - 175f;
        if (centered_z > 5f)
        {
            if (dir == Direction.Left)
                _sprite.Rotate(0f, 0f, 270f * Time.deltaTime);
            else
                _sprite.Rotate(0f, 0f, -270f * Time.deltaTime);
        }
    }

    public void Damaged()
    {
        _animator.SetTrigger("Anim_Hit");
        hp -= 25f;
        
        if (hp <= 0f)
        {
            Dead();
        }
    }
    private void Dead()
    {
        _animator.SetTrigger("Anim_Dead");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerAttackCollider"))
        {
            Damaged();
        }
    }

}