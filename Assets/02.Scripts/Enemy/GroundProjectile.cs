using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundProjectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _time = 0f;
    private CircleCollider2D _collider;
    public Transform _groundCheckPos;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = transform.GetComponentInChildren<CircleCollider2D>();
        _groundCheckPos = transform.GetComponentInChildren<Transform>();
        Assert.IsNotNull(_rb);
        Assert.IsNotNull(_collider);
        Assert.IsNotNull(_groundCheckPos);
    }

    private void OnEnable()
    {
        _time = 0f;
    }

    private void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;
        float calcTime = _time * 2f;
        _rb.velocity = new Vector2(Mathf.Min( Mathf.Pow(calcTime, 3f) * 5f , 30f ), 0f);
        Collider2D hit = Physics2D.OverlapCircle(_groundCheckPos.position, .5f, LayerMask.GetMask("Ground"));
        if(hit == null)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_groundCheckPos.position, .5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().Damaged(20f);
        }
    }
}
