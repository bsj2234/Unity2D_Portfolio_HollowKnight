using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

//버튼에 클릭시 플레이어 인벤검사 후 
//장착/해제 실행
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

    //버튼을 눌렀을떄 부적이 있으면 장착을 해제한다
    public void DoEquipUnequip()
    {
        unequippedItem = GameManager.Instance.GetPlayer().EquipUnequipCharm(EquipIndex);
        UiManager.Instance.inventoryUi.RefreshAll();
    }

    public void Refresh()
    {
        Init();
        unequippedItem = GameManager.Instance.GetPlayer().EquppedCharmAt(EquipIndex);
        if (unequippedItem != null)
        {
            _button.image.sprite = unequippedItem.CharmType.Icon;
            ((RectTransform)(_button.transform)).sizeDelta = new Vector2(80f, 80f);
        }
        else
        {
            _button.image.sprite = DefaultSprite;
            ((RectTransform)(_button.transform)).sizeDelta = new Vector2(40f, 40f);
        }

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
