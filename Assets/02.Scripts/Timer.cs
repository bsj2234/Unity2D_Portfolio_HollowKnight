using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [field: SerializeField] private float _timeStamp = 0f;
    [field: SerializeField] public float Duration = 0f;
    [field: SerializeField] bool IsPossible = false;

    public void SetTimer(float duration)
    {
        Duration = duration;
        _timeStamp = Time.time;
    }

    public bool IsTimeOver()
    {
        if(Time.time > _timeStamp + Duration)
        {
            return true;
        }
        return false;
    }

    public void ResetTime()
    {
        _timeStamp = Time.time;
    }
}
