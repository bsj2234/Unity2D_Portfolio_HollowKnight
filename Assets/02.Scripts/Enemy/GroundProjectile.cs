using System;
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
        float powTime = Mathf.Pow(calcTime, 3f);
        _rb.velocity = Mathf.Min(powTime * 5f , 30f ) * transform.right;
        transform.localScale = new Vector3(Mathf.Min(powTime * 3f, 1f),1f,1f);
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
}
