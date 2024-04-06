using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Followtarget : MonoBehaviour
{
    public Transform Target;
    public float Speed;

    private float cameraOffset = -10f;
    private Vector3 currentVelocity = Vector3.zero;
    void LateUpdate()
    {
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(Target.position.x, Target.position.y, cameraOffset), ref currentVelocity, Speed);
        transform.position = Vector3.Lerp(transform.position  , new Vector3(Target.position.x, Target.position.y, cameraOffset), Time.deltaTime*Speed);
        
    }

}
