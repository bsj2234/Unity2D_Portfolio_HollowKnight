
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
//�׷��ϱ� ������ ȿ���� �����ٵ�
//ȿ���� ��뤾
//�κ��丮 �ϴ� ������ �ֿ�� ȹ������
//�������� �ֿ�����
//�������� �Ӱ��� ����� ����
//owner�� ���� �������� owner�� ������ owner�� ��ȭ��
//
//������ �ν������� ����

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
