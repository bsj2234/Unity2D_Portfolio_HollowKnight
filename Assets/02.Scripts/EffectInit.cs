using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInit : MonoBehaviour
{
    private SpriteRenderer sprite;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        sprite.enabled = true;
    }
}
