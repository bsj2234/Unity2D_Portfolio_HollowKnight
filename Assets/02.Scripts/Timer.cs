using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [field: SerializeField] private float _countStarted = 0f;

    [field: SerializeField] public float Duration = 0f;
    // ~하는 시간동안 
    [field: SerializeField] bool IsPossible = false;
}
