

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory : ScriptableObject
{
    public List<ItemInstance> items = new();

    public void AddItem(ItemInstance itemToAdd)
    {
        items.Add(itemToAdd);
    }

    public void RemoveItem(ItemInstance itemToRemove)
    {
        items.Remove(itemToRemove);
    }


    [CreateAssetMenu]
    public class FixedInventoryItem : ScriptableObject
    {
        public bool hasItem;
        public string itemName;
        public Sprite icon;
        [TextArea]
        public string description;

    }

}