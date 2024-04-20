using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UiMp : MonoBehaviour
{
    public float mp = 0f;
    public float minMp = 0f;
    public float maxMp = 100f;
    public RectTransform MaskOrb;
    public RectTransform OrbImage;

    //MP를 나타내기 위한 마스크의 위치 
    public float minMaskOffset = -65f;
    public float maxMaskOffset = 0f;
    public Vector2 InitialPosMask;
    public Vector2 InitialPosImage;
    public void UpdateUi()
    {
        mp = GameManager.Instance.GetPlayer().GetMp();

        float AddPos = Mathf.Lerp(minMaskOffset, maxMaskOffset, mp / maxMp);
        MaskOrb.anchoredPosition = new Vector2(InitialPosMask.x, InitialPosMask.y + AddPos);
        OrbImage.anchoredPosition = new Vector2(InitialPosImage.x, InitialPosImage.y - AddPos);
    }
    private void Awake()
    {
        InitialPosMask = MaskOrb.anchoredPosition;
        InitialPosImage = OrbImage.anchoredPosition;
    }

    private void Start()
    {
        UpdateUi();
    }
}
