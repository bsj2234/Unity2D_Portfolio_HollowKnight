using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;

public class ReturningProjectile : MonoBehaviour
{
    private Vector2 _initialPosition;
    public float Distance = 10f;
    public Direction direction = Direction.Right;
    private Rigidbody2D _rigidbody;
    private float _lerpPos = 0f;
    private float _time = 0f;
    private void Awake()
    {

        _rigidbody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull( _rigidbody );
        _initialPosition = transform.position;
    }

    private void Update()
    {

        //Todo애니메이션으로 만드는게 좋을듯
        _time += Time.deltaTime;
        if(_time > 2f)
        {
            gameObject.SetActive(false);
        }
        _lerpPos = (_time < 1f) ? _time : 1f -_time + 1f; 
        _rigidbody.MovePosition(Vector2.Lerp(_initialPosition, _initialPosition + direction.Get2D() * Distance, _lerpPos));
    }
}
