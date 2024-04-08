using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class EnemyBossKnightController : MonoBehaviour
{
    public Vector2 moveDirInput;

    //관련 컴포넌트들
    private EnemyBossKnight _player;

    //입력관련 상태
    //프로퍼티다니까 인스펙터에 안보임
    //나중에 get이나 set에 조건이 필요할떄 프로퍼티랑 SerializeField를 사용해서 프로퍼티화 하는게 좋지 않나?
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
