using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class MoveComponent : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Vector2 inputVector = Vector2.zero;
    public float nonInputDrag = 10f;
    public float inAirDrag = 1f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(_rigidbody);
    }

    //Check Maxspeed And Move
    //If OverMaxSpeed Add Force?
    public void MoveUpdate(Vector2 moveVector, float maxSpeed)
    {
        //움직임 현재 속도를 체크하고
        //입력받은 방향으로 움직어야함
        //Todo
        //_rigidbody.drag = inAirDrag;
        moveVector.y = 0f;
        Vector2 curVelocity = _rigidbody.velocity;
        //최대 속도를 넘으면 입력 못받게
        //if(IsOverMaxSpeed(curVelocity, maxSpeed))
        //{
        //    if(VectorExtender.IsOpposite(moveVector, curVelocity))
        //    {
        //        //_rigidbody.drag = inAirDrag * 2f;
        //        Vector2 toSub = maxSpeed * moveVector * Time.deltaTime * 10f;
        //        _rigidbody.velocity = curVelocity + toSub;
        //    }
        //}
        //else
        //{
        //  _rigidbody.velocity = new Vector2(moveVector.x * maxSpeed, _rigidbody.velocity.y);
        //}
        _rigidbody.velocity = new Vector2(moveVector.x * maxSpeed, _rigidbody.velocity.y);
    }
    //    private bool IsOverMaxSpeed(Vector2 currentSpeed, float maxSpeed)
    //    {
    //        return currentSpeed.magnitude > maxSpeed;
    //    }
    //
}

public static class VectorExtender
{
    public static bool IsOpposite(this Vector2 v1, Vector2 v2)
    {
        return (Vector2.Dot(v1, v2) < 0f) ? true : false;
    }
}
