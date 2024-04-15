using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUi : MonoBehaviour
{
    private Image _image;
    private TMP_Text _text;
    private Shop _shop;
    private CharmInstance _charm;
    private int _index;
    private void Awake()
    {
        
        _image = GetComponent<Image>();
        _text = transform.GetComponentInChildren<TMP_Text>();
    }

    public void UpdateUi(Shop shop ,Shop.ShopCharm shopCharm, int index)
    {
        _shop = shop;
        _image.sprite = shopCharm.CharmInstance.CharmType.Icon;
        _text.text = shopCharm.ItemCost.ToString();
        if(shopCharm.Sold)
            _image.color = Color.black;
        else
            _image.color = Color.white;

    }

    public void OnClick()
    {
        _shop.TrySell(GameManager.Instance.Player, _index);
    }
}
