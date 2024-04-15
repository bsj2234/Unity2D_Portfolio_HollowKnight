
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public DynamicInventory inventory;
    public ItemDisplay[] slots;

    private void Start()
    {
        UpdateInventory();
    }

    private void UpdateInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].UpdateItemDisplay(inventory.items[i].CharmType.Icon, i);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    public void DropItem(int itemIndex)
    {
        //떨어트릴 아이템 생성
        GameObject droppedItem = new GameObject();
        droppedItem.AddComponent<Rigidbody>();
        droppedItem.AddComponent<InstanceItemContainer>().item = inventory.items[itemIndex];
        //GameObject itemModel = Instantiate(inventory.items[itemIndex].CharmType., droppedItem.transform);

        //인벤토리에서 아이템 삭제
        inventory.items.RemoveAt(itemIndex);

        //업데이트
        UpdateInventory();
    }

}