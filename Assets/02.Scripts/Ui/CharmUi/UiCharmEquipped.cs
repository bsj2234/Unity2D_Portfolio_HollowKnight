using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

//��ư�� Ŭ���� �÷��̾� �κ��˻� �� 
//����/���� ����
public class UiCharmEquipped : MonoBehaviour
{
    public int EquipIndex = -1;
    public Sprite DefaultSprite;
    private Button _button;
    private CharmInstance unequippedItem;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if(_button == null)
        {
            _button = GetComponent<Button>();
            Assert.IsNotNull(_button);
            _button.onClick.AddListener(DoEquipUnequip);
            Assert.IsTrue(EquipIndex != -1, "EquipIndexNotSet");
            Assert.IsNotNull(DefaultSprite);
        }
    }

    //��ư�� �������� ������ ������ ������ �����Ѵ�
    public void DoEquipUnequip()
    {
        unequippedItem = GameManager.Instance.GetPlayer().EquipUnequipCharm(EquipIndex);
        _button.image.sprite = (unequippedItem != null) ? unequippedItem.CharmType.Icon : DefaultSprite;
        UiManager.Instance.inventoryUi.RefreshAll();
    }

    public void Refresh()
    {
        Init();
        unequippedItem = GameManager.Instance.GetPlayer().EquppedCharmAt(EquipIndex);
        _button.image.sprite = (unequippedItem != null) ? unequippedItem.CharmType.Icon : DefaultSprite;

    }

    public void OnMouseEnter()
    {
        unequippedItem = GameManager.Instance.GetPlayer().EquppedCharmAt(EquipIndex);
        if ( unequippedItem != null )
        {
            UiManager.Instance.inventoryUi.SetDescription(unequippedItem);
        }
    }
}
