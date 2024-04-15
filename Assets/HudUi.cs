using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudUi : MonoBehaviour
{
    [SerializeField] private UiHp hp;
    [SerializeField] private UiMp mp;
    [SerializeField] private UiCoin coin;
    public void RefreshAll()
    {
        hp.UpdateUi();
        mp.UpdateUi();
        coin.UpdateUi();
    }
}
