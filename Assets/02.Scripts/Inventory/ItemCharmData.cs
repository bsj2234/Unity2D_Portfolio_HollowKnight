
//https://gamedevbeginner.com/how-to-make-an-inventory-system-in-unity/#overview
using UnityEngine;

[CreateAssetMenu]
public class ItemCharmData : ItemData
{
    public int Slot = 1;
    [TextArea] 
    public ICharmEffect Effect;
}
//그러니까 아이템 효과가 있을텐데
//효과를 사용ㅎ
//인벤토리 일단 부적을 주우면 획득하지
//아이템을 주웠을때
//아이템으 ㅣ가진 기능을 실행
//owner를 통해 아이템이 owner에 접근해 owner를 강화함
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