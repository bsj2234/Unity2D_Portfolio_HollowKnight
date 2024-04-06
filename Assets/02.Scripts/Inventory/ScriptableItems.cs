
//https://gamedevbeginner.com/how-to-make-an-inventory-system-in-unity/#overview
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject model;
    [TextArea]
    public string description;
}

[System.Serializable]
public class ItemInstance
{
    public ItemData itemType;
    public int condition;
    public int ammo;

    public ItemInstance(ItemData itemData)
    {
        itemType = itemData;
    }
}

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

[CreateAssetMenu]
public class DynamicInventory : ScriptableObject
{
    public int maxItems = 10;
    public List<ItemInstance> items = new();

    public bool AddItem(ItemInstance itemToAdd)
    {
        //���� �������� ������ �� ���� �߰�
        //Todo ���߿� ĭ���� �ִ� ������ ����
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == itemToAdd)
            {
                items[i] = itemToAdd;
                return true;
            }
        }

        //�κ��� ���� ���������
        if (items.Count < maxItems)
        {
            items.Add(itemToAdd);
            return true;
        }

        Debug.Log("No space in inventory");
        return false;
    }
}