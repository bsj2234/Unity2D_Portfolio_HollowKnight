using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shop : MonoBehaviour, IInteractable
{
    public GameObject InteractUi;
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
        //ºóÄ­ÀÌ°Å³ª
        if (ShopInventory[index] == null)
            return null;
        //µ·ºÎÁ·ÇÏ°Å³ª
        if (player.coinCount < ShopInventory[index].ItemCost)
            return null;
        //ÆÈ·È°Å³ª
        if (ShopInventory[index].Sold)
            return null;
        CharmInstance currentItem = ShopInventory[index].CharmInstance;
        ShopInventory[index].Sold = true;
        player.AddItem(currentItem);
        player.AddCoin(-ShopInventory[index].ItemCost);
        return currentItem;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractUi.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractUi.SetActive(false);
        }
    }
}
