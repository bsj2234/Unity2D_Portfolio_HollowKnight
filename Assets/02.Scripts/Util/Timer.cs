using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimeCounter : MonoBehaviour
{ 
    public static float sharedCounter;
    public float Counter;
    public TimeCounter()
    {
    }

    void Update()
    {
        sharedCounter -= Time.deltaTime;
        Counter += Time.deltaTime;
    }

    public void Clear()
    { Counter = 0f; }
    public void ShaeredTimerClear()
    {
        sharedCounter = 0f;
    }

}
