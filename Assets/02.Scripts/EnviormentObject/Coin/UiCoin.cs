using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiCoin : MonoBehaviour
{
    public TMP_Text text;
    public void UpdateUi()
    {
        text.text = GameManager.Instance.GetPlayer().GetMoney().ToString();
    }
}
