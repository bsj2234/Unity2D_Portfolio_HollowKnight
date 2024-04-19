using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private bool _triggered = false;
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _initalPos;
    [SerializeField] Vector3 vel;
    [SerializeField] Vector3 targetPos => _target.transform.position;
    [SerializeField] float smoothTime = .3f;
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _initalPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (_triggered)
        {
            Vector3.SmoothDamp(transform.position, targetPos, ref vel, smoothTime);
            _rigidbody.velocity = vel;
        }
        else
        {
            Vector3.SmoothDamp(transform.position, _initalPos, ref vel, smoothTime);
            _rigidbody.velocity = vel;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _triggered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _triggered = false;
        }
    }
}
