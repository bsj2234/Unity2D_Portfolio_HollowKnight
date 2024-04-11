using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveDirInput;

    //���� ������Ʈ��
    private Player _player;
    private UiManager _uiManager;
    //�Է°��� ����
    //������Ƽ�ٴϱ� �ν����Ϳ� �Ⱥ���
    //���߿� get�̳� set�� ������ �ʿ��ҋ� ������Ƽ�� SerializeField�� ����ؼ� ������Ƽȭ �ϴ°� ���� �ʳ�?
    public bool hasMoveInput = false;

    private bool _playerControl = true;




    private void Awake()
    {
        _player = GetComponent<Player>();

        Assert.IsNotNull(_player);

        _uiManager = UiManager.Instance;
        Assert.IsNotNull(_uiManager);
    }


    private void FixedUpdate()
    {
        if (_playerControl == false)
        { return; }
        _player.Move(moveDirInput);
    }


    //OnInput
    private void OnMove(InputValue inputValue)
    {
        if (_playerControl == false)
        { return; }
        moveDirInput = inputValue.Get<Vector2>();
        hasMoveInput = Mathf.Abs(moveDirInput.x) > .1f;
    }
    private void OnJump(InputValue inputValue)
    {
        if (_playerControl == false)
        { return; }
        if (inputValue.isPressed == true)
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
        if (_playerControl == false)
        { return; }
        if (inputValue.isPressed == false)
        {
            _player.Attack(GetAttackDir());
        }
    }
    private void OnDodge(InputValue inputValue)
    {
        if (_playerControl == false)
        { return; }
        if (inputValue.isPressed == false)
        {
            _player.Dodge();
        }
    }

    private void OnInteract(InputValue inputValue)
    {
        if(_playerControl == false)
        { return; }
        if(inputValue.isPressed != false)
        {
            _player.TryInteract();
        }
    }    

    private void OnInventory(InputValue inputValue)
    {
        if(inputValue.isPressed != false)
        {
            if(_playerControl)
            {
                _playerControl = false;
                _uiManager.InventoryOn();
            }
            else
            {
                _playerControl = true;
                _uiManager.InventoryOff();
            }
        }
    }

    public Vector2 GetAttackDir()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 playerToMouse = -(Vector2)_player.transform.position + mousePosition;
        playerToMouse = playerToMouse.normalized;

        //���ݹ��� ����
        if (playerToMouse.x > .7f)
        {
            return Vector2.right;
        }
        if (playerToMouse.x < -.7f)
        {
            return Vector2.left;
        }
        if (playerToMouse.y > .7f)
        {
            return Vector2.up;
        }
        if(playerToMouse.y < -.7f)
        {
            return Vector2.down;
        }
        Assert.IsTrue(false, "can not find mouse dir vector");
        return Vector2.zero;
    }

    public Vector2 GetLookDir()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 playerToMouse = -(Vector2)_player.transform.position + mousePosition;
        playerToMouse = playerToMouse.normalized;

        //���ݹ��� ����
        if (playerToMouse.x > .7f)
        {
            return Vector2.right;
        }
        if (playerToMouse.x < -.7f)
        {
            return Vector2.left;
        }
        if (playerToMouse.y > .7f)
        {
            return Vector2.up;
        }
        if (playerToMouse.y < -.7f)
        {
            return Vector2.down;
        }
        Assert.IsTrue(false, "can not find mouse dir vector");
        return Vector2.zero;
    }
}
