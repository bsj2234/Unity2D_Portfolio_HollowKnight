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

    public void AllOff()
    {
        inventoryUi.InventoryOff();
        //hudUi.SetActive(false);
        shopUi.SetActive(false);
    }

    public void DeadUiOn()
    {
        deadUi.DeadUiOn();
    }
    public void DeadUiOff()
    {
        deadUi.DeadUiOff();
    }
}
