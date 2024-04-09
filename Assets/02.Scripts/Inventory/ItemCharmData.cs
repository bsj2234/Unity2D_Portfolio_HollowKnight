
//https://gamedevbeginner.com/how-to-make-an-inventory-system-in-unity/#overview
using UnityEngine;

[CreateAssetMenu]
public class ItemCharmData : ItemData
{
    public int Slot = 1;
    [TextArea] 
    public ICharmEffect Effect;
}
//�׷��ϱ� ������ ȿ���� �����ٵ�
//ȿ���� ��뤾
//�κ��丮 �ϴ� ������ �ֿ�� ȹ������
//�������� �ֿ�����
//�������� �Ӱ��� ����� ����
//owner�� ���� �������� owner�� ������ owner�� ��ȭ��
//


[System.Serializable]
public class CharmInstance
{
    public ItemCharmData CharmType;

    public CharmInstance(ItemCharmData charmData)
    {
        CharmType = charmData;
    }
}

public interface ICharmEffect
{
    void OnEquip();
}

public class WaywardCompassCharm : ICharmEffect
{
    public ItemCharmData data;
    private Player _owner;

    public WaywardCompassCharm(Player owner)
    {
        _owner = owner;
    }
    public void OnEquip()
    {
        _owner.SetVisibleSelfOnMiniMap(true);
    }

}
public class GatheringSwarm : ICharmEffect
{
    public ItemCharmData data;
    private Player _owner;
    public void OnEquip()
    {
        //GetNearGeoToTarget(true);
    }
}
public class StalwartShellCharm : ICharmEffect
{
    public ItemCharmData data;
    private Player _owner;
    public void OnEquip()
    {
        //HardenBody(true);
    }
}