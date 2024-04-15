
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DynamicInventory : ScriptableObject
{
    public int maxItems = 10;
    public List<CharmInstance> items = new List<CharmInstance>();

    public bool AddItem(CharmInstance itemToAdd)
    {
        //���� �������� ������ �� ���� �߰�
        //Todo ���߿� ĭ���� �ִ� ������ ����
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == itemToAdd)
            {
                items[i] = itemToAdd;
                Debug.Log($"ItemAdded {itemToAdd.CharmType.ItemName}");
                foreach (var item in items)
                {

                    Debug.Log($"Has: {itemToAdd.CharmType.ItemName}");
                }
                return true;
            }
        }

        //�κ��� ���� ���������
        if (items.Count < maxItems)
        {
            items.Add(itemToAdd);
            Debug.Log($"ItemAdded {itemToAdd.CharmType.ItemName}");
            foreach (var item in items)
            {

                Debug.Log($"Has: {itemToAdd.CharmType.ItemName}");
            }
            return true;
        }

        Debug.Log("No space in inventory");
        return false;
    }
}
