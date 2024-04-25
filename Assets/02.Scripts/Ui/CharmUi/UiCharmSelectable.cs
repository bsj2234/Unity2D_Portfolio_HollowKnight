using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

//��ư�� Ŭ���� �÷��̾� �κ��˻� �� 
//����/���� ����
//������ ĭ�� ������Ʈ �Ǿ����.
//�̺�Ʈ �����
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

    //��ư�� �������� ������ ������ ������ �����Ѵ�
    public void DoEquip()
    {
        //������ ���� ������ null��
        GameManager.Instance.GetPlayer().TryEquipCharm(CharmIndex);
        inventory.RefreshAll();
    }
    public void Refrash()
    {
        Init();
        currentCharm = GameManager.Instance.GetPlayer().CharmAt(CharmIndex);
        bool isEquipped = GameManager.Instance.GetPlayer().IsItemEquipped(currentCharm);
        if (currentCharm != null && !isEquipped)
        {
            _button.image.sprite = currentCharm.CharmType.Icon;
            ((RectTransform)(_button.transform)).sizeDelta = new Vector2(60f, 60f);
        }
        else
        {
            _button.image.sprite = DefaultSprite;
            ((RectTransform)(_button.transform)).sizeDelta = new Vector2(40f, 40f);
        }
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