using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

//버튼에 클릭시 플레이어 인벤검사 후 
//장착/해제 실행
//장착된 칸이 업데이트 되어야함.
//이벤트 만들까
public class UiCharmSelectable : MonoBehaviour
{
    public InventoryUi inventory;

    public int CharmIndex = -1;
    public Sprite DefaultSprite;
    private Button _button;
    private CharmInstance currentCharm;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
            Assert.IsNotNull(_button);
            _button.onClick.AddListener(DoEquip);
            Assert.IsTrue(CharmIndex != -1, "EquipIndexNotSet");
            Assert.IsNotNull(DefaultSprite);
        }
    }

    //버튼을 눌렀을떄 부적이 있으면 장착을 해제한다
    public void DoEquip()
    {
        //아이템 장착 성공시 null로
        GameManager.Instance.GetPlayer().TryEquipCharm(CharmIndex);
        inventory.RefreshAll();
    }
    public void Refrash()
    {
        Init();
        currentCharm = GameManager.Instance.GetPlayer().CharmAt(CharmIndex);
        bool isEquipped = GameManager.Instance.GetPlayer().IsItemEquipped(currentCharm);
        _button.image.sprite = (currentCharm != null && !isEquipped) ? currentCharm.CharmType.Icon : DefaultSprite;
    }

    public void OnMouseEnter()
    {
        currentCharm = GameManager.Instance.GetPlayer().CharmAt(CharmIndex);
        if(currentCharm != null )
        {
            UiManager.Instance.inventoryUi.SetDescription(currentCharm);
        }
    }
}
