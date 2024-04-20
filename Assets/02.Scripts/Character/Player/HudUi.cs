using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudUi : MonoBehaviour
{

    public GameObject _HudUiGameObject;
    [SerializeField] private UiHp hp;
    [SerializeField] private UiMp mp;
    [SerializeField] private UiCoin coin;
    public void RefreshAll()
    {
        hp.UpdateUi();
        mp.UpdateUi();
        coin.UpdateUi();
    }

    internal void SetActive(bool v)
    {
        _HudUiGameObject.SetActive(v);
    }
}
