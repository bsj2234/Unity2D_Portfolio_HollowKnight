using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class EnemyBossKnightController : MonoBehaviour
{
    public Vector2 moveDirInput;

    //���� ������Ʈ��
    private EnemyBossKnight _player;

    //�Է°��� ����
    //������Ƽ�ٴϱ� �ν����Ϳ� �Ⱥ���
    //���߿� get�̳� set�� ������ �ʿ��ҋ� ������Ƽ�� SerializeField�� ����ؼ� ������Ƽȭ �ϴ°� ���� �ʳ�?
    public bool hasMoveInput = false;




    private void Start()
    {
        _player = GetComponent<EnemyBossKnight>();

        Assert.IsNotNull(_player);
    }


    private void Update()
    {
        _player.Move(moveDirInput);
    }


}
