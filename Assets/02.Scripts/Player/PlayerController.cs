using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveDirInput;

    //���� ������Ʈ��
    private Player _player;

    //�Է°��� ����
    //������Ƽ�ٴϱ� �ν����Ϳ� �Ⱥ���
    //���߿� get�̳� set�� ������ �ʿ��ҋ� ������Ƽ�� SerializeField�� ����ؼ� ������Ƽȭ �ϴ°� ���� �ʳ�?
    public bool hasMoveInput = false;




    private void Start()
    {
        _player = GetComponent<Player>();

        Assert.IsNotNull(_player);
    }


    private void FixedUpdate()
    {
        _player.Move(moveDirInput);
    }


    //OnInput
    private void OnMove(InputValue inputValue)
    {
        moveDirInput = inputValue.Get<Vector2>();
        hasMoveInput = Mathf.Abs(moveDirInput.x) > .1f;
    }
    private void OnJump(InputValue inputValue)
    {
        if(inputValue.isPressed == true)
        {
            _player.StartJump();
        }
        else
        {
            _player.EndJump();
        }
    }
    private void OnAttack(InputValue inputValue)
    {
        if (inputValue.isPressed == false)
        {
            _player.Attack();
        }
    }
    private void OnDodge(InputValue inputValue)
    {
        if (inputValue.isPressed == false)
        {
            _player.Dodge();
        }
    }

    private void OnInteract(InputValue inputValue)
    {
        if(inputValue.isPressed != false)
        {
            _player.TryInteract();
        }
    }    


}
