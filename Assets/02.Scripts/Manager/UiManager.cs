using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : Singleton<UiManager>
{
    public InventoryUi inventoryUi;
    public HudUi hudUi;
    public ShopUi shopUi;
    public DeadUi deadUi;
    public PauseUi PauseUi;

    public bool IsInUiMode { get; internal set; } = false;

    public void RefreshInventory()
    {
        inventoryUi.RefreshAll();
    }
    public void RefreshHud()
    {
        hudUi.RefreshAll();
    }
    public void RefreshShop()
    {
        shopUi.RefreshAll();
    }

    private void AllOff()
    {
        inventoryUi.InventoryOff();
        //hudUi.SetActive(false);
        shopUi.SetActive(false);
        PauseUi.ActiveUi = false;
    }

    public void DeadUiOn()
    {
        deadUi.DeadUiOn();
    }
    public void DeadUiOff()
    {
        deadUi.DeadUiOff();
    }

    public void ExitUiMode()
    {
        AllOff();
        IsInUiMode = false;
        GameManager.Instance.Player._controller._playerControlable = true;
    }

    public void PauseOn()
    {
        IsInUiMode = true;
        PauseUi.ActiveUi = true;
    }

    public void InventoryOn()
    {
        IsInUiMode = true;
        inventoryUi.InventoryOn();
    }
    public void ShopUiOn(Shop shop)
    {
        IsInUiMode = true;
        shopUi.Init(shop);
        shopUi.SetActive(true);
    }
}
