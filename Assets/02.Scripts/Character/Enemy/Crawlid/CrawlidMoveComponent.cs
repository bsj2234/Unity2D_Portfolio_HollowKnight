using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Windows;


public class CrawlidMoveComponent : Character, IFightable
{
    public Direction dir = Direction.Left;
    public float rayOffset = .5f;
    public float rayDistance = .1f;
    public LayerMask ground = 1 << 7;
    public float MaxSpeed = 5f;
    public bool isGrounded = true;

    public CombatComponent combat;

    public CircleCollider2D circleCollider;

    private Transform _sprite;
    private Animator _animator;
    public Rigidbody2D rb;
    private float _rotatedTime;
    private Collision2D _collision;

    //combat

    private void Awake()
    {
        _sprite = transform.GetChild(0);
        Assert.IsNotNull(_sprite);
        _animator = _sprite.GetComponent<Animator>();
        Assert.IsNotNull(_animator);

        combat.Init(transform);
        combat.OnDead += OnDead;
    }

    private void FixedUpdate()
    {
        MoveForward();
    }
    public void MoveForward()
    {
        if(combat.IsDead())
        {
            rb.velocity = Vector3.zero;
            rb.gravityScale = 1f;
            return;
        }
        //gravity
        rb.velocity = 3f * -transform.up;
        //check
        //내 위치 왼쪽 오프셋부터, 아래쪽 방향, 거리
        RaycastHit2D ground_hit = Physics2D.Raycast(transform.position, -transform.up, rayDistance, ground);
        RaycastHit2D behind_ground_hit = Physics2D.Raycast(transform.position + (-transform.right) * -rayOffset, -transform.up, rayDistance, ground);
        //내 위치 , 왼쪽 방향, 거리
        RaycastHit2D front_hit = Physics2D.Raycast(transform.position, -transform.right, rayDistance + circleCollider.radius, ground);
        Debug.DrawRay(transform.position + (-transform.right) * rayOffset, -transform.up * rayDistance, Color.red, 0.01f);

        //rotaton edge
        if (ground_hit.collider == null && behind_ground_hit.collider != null && Time.time - _rotatedTime > .1f)
        {
            _rotatedTime = Time.time;
            transform.Rotate(0f, 0f, 90f);
            _sprite.Rotate(0f, 0f, -90f);
        }
        //rotate on wall
        if (front_hit.collider != null && Time.time - _rotatedTime > .1f)
        {
            _rotatedTime = Time.time;
            transform.Rotate(0f, 0f, -90f);
            _sprite.Rotate(0f, 0f, 90f);
        }
        //move
        if (behind_ground_hit.collider != null || ground_hit.collider!= null)
        {
            rb.AddForce(3f * -transform.right, ForceMode2D.Impulse);
        }
        //slow rotate
        //0 ~ 5
        //left 270  right 90
        //     90         -90
        //     0 == -180
        float centered_z = _sprite.localEulerAngles.z - 180f;
        if (centered_z > 5f)
        {
            if (_sprite.localEulerAngles.z > 5f)
                _sprite.Rotate(0f, 0f, 270f * Time.deltaTime);
        }
        else if (centered_z <= -5f)
        {
            if (_sprite.localEulerAngles.z > 5f) 
                _sprite.Rotate(0f, 0f, -270f * Time.deltaTime);
        }

    }
    public void OnDamaged()
    {
        _animator.SetTrigger("Anim_Hit");
    }
    private void OnDead()
    {
        _animator.SetTrigger("Anim_Dead");
        Destroy(gameObject, 5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(combat.IsDead())
            return;
        if (collision.CompareTag("PlayerAttackCollider"))
        {
            combat.TakeDamage(collision.transform.position, 10f);
        }
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.Player.GetCombatComponent().TakeDamage(transform.position, 1f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        _collision = collision;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _collision = null;
    }

    public override CombatComponent GetCombatComponent()
    {
        return combat;
    }
}