
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DynamicInventory : ScriptableObject
{
    public int maxItems = 10;
    public List<ItemInstance> items = new List<ItemInstance>();

    public bool AddItem(ItemInstance itemToAdd)
    {
        //���� �������� ������ �� ���� �߰�
        //Todo ���߿� ĭ���� �ִ� ������ ����
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

        //�κ��� ���� ���������
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
