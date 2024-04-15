using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shop : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class ShopCharm
    {
        public int ItemCost;
        public CharmInstance CharmInstance;
        public bool Sold;
    }
    public List<ShopCharm> ShopInventory = new List<ShopCharm>();

    public void Interact(Player player)
    {
        player.OpenShopHud(this);
    }

    public CharmInstance TrySell(Player player, int index)
    {
        //��ĭ�̰ų�
        if (ShopInventory[index] == null)
            return null;
        //�������ϰų�
        if (player.coinCount < ShopInventory[index].ItemCost)
            return null;
        //�ȷȰų�
        if (ShopInventory[index].Sold)
            return null;
        CharmInstance currentItem = ShopInventory[index].CharmInstance;

        ShopInventory[index].Sold = true;
        player.AddItem(currentItem);
        return currentItem;
    }
}
