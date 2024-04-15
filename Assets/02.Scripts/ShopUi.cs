using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUi : MonoBehaviour
{
    public List<Shop.ShopCharm> shopCharms;
    public List<ShopItemUi> buttonList;
    public Shop _shop;
    public GameObject _itemPrefab;
    public Transform buttonParent;
    public void Init(Shop shop)
    {
        shopCharms = shop.ShopInventory;
    }

    public void SetActive(bool v)
    {
        gameObject.SetActive(v);
    }

    public void RefreshAll()
    {
        for (int i = 0; i < shopCharms.Count; i++)
        {
            if (buttonList.Count < shopCharms.Count)
            {
                GameObject child = Instantiate(_itemPrefab);
                RectTransform rect = child.GetComponent<RectTransform>();
                child.transform.SetParent(buttonParent);
                rect.anchoredPosition = new Vector2(0f, (float)(-25 - i * 150));
                rect.sizeDelta = new Vector2(100f, 100f);
                buttonList.Add(child.GetComponent<ShopItemUi>());
            }
            buttonList[i].UpdateUi(_shop, shopCharms[i], i);
        }
    }

    private void OnEnable()
    {
        RefreshAll();
    }
}
