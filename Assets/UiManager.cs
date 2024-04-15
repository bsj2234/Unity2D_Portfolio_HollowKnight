using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : Singleton<UiManager>
{
    public InventoryUi inventoryUi;
    public HudUi hudUi;
    public ShopUi shopUi;

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
}
