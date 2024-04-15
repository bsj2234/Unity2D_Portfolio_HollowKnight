
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DynamicInventory : ScriptableObject
{
    public int maxItems = 10;
    public List<ItemInstance> items = new List<ItemInstance>();

    public bool AddItem(ItemInstance itemToAdd)
    {
        //같은 아이템이 있으면 그 수만 추가
        //Todo 나중에 칸별로 최대 아이템 제한
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == itemToAdd)
            {
                items[i] = itemToAdd;
                Debug.Log($"ItemAdded {itemToAdd.itemType.itemName}");
                foreach (var item in items)
                {

                    Debug.Log($"Has: {itemToAdd.itemType.itemName}");
                }
                return true;
            }
        }

        //인벤에 공간 있으면넣음
        if (items.Count < maxItems)
        {
            items.Add(itemToAdd);
            Debug.Log($"ItemAdded {itemToAdd.itemType.itemName}");
            foreach (var item in items)
            {

                Debug.Log($"Has: {itemToAdd.itemType.itemName}");
            }
            return true;
        }

        Debug.Log("No space in inventory");
        return false;
    }
}
