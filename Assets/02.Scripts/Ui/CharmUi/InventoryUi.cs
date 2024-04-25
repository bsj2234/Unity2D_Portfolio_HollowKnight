
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class InventoryUi : MonoBehaviour
{
    private UiCharmSelectable[] _selectables;
    private UiCharmEquipped[] _equppeds;
    public GameObject _InventoryUiGameObject;
    public GameObject HudUi;
    public TMP_Text CharmDescName;
    public TMP_Text CharmSlot;
    public TMP_Text CharmBroken;
    public TMP_Text CharmDesc;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (_selectables == null || _equppeds == null)
        {
            _selectables = GetComponentsInChildren<UiCharmSelectable>();
            _equppeds = GetComponentsInChildren<UiCharmEquipped>();
        }
        //Wow ������  FindObjectsOfTypeAll�� �������� ���� �����յ� ��𿡼� �����´� �ű��ϴ�
        // ������������� ���� �ʴ�

        Assert.IsNotNull(_selectables);
        Assert.IsNotNull(_equppeds);
    }

    public void RefreshAll()
    {
        Init();
        foreach (UiCharmSelectable selectable in _selectables )
        {
            if(selectable == null)
            {
                continue;
            }
            if( selectable.CharmIndex != -1 )
            {
                selectable.Refrash();
            }
        }
        foreach (UiCharmEquipped equipped in _equppeds )
        {
            if (equipped == null)
            {
                continue;
            }
            if (equipped.EquipIndex != -1)
            {
                equipped.Refresh();

            }
        }
    }

    public void InventoryOn()
    {
        RefreshAll();
        _InventoryUiGameObject.SetActive(true);
        HudUi.SetActive(false);
    }
    public void InventoryOff()
    {
        _InventoryUiGameObject.SetActive(false);
        HudUi.SetActive(true);
    }

    public void SetActive(bool v)
    {
        _InventoryUiGameObject.SetActive(v);
    }

    public void SetDescription(CharmInstance charm)
    {
        CharmDescName.text = charm.CharmType.ItemName;
        CharmSlot.text = $"����: {charm.CharmType.Slot}";
        CharmBroken.text = charm.CharmType.Broken? "�ı���":"";
        CharmDesc.text = charm.CharmType.Description;
    }
}
