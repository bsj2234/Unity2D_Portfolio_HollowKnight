
//https://gamedevbeginner.com/how-to-make-an-inventory-system-in-unity/#overview
using UnityEngine;

[CreateAssetMenu]
public class CharmData : ScriptableObject
{
    public string ItemName;
    public Sprite Icon;
    public Sprite Broken;
    public int Slot = 1;
    [TextArea]
    public string Description;

}
//그러니까 아이템 효과가 있을텐데
//효과를 사용ㅎ
//인벤토리 일단 부적을 주우면 획득하지
//아이템을 주웠을때
//아이템으 ㅣ가진 기능을 실행
//owner를 통해 아이템이 owner에 접근해 owner를 강화함
//
//부적이 부숴질수도 있음

[System.Serializable]
public class CharmInstance
{
    public CharmData CharmType;
    public bool Braked = false;
    public int RepairFee = 0;
    public CharmInstance(CharmData charmData)
    {
        CharmType = charmData;
    }
}
