using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveDirInput;

    //관련 컴포넌트들
    private Player _player;
    private UiManager _uiManager;
    //입력관련 상태
    //프로퍼티다니까 인스펙터에 안보임
    //나중에 get이나 set에 조건이 필요할떄 프로퍼티랑 SerializeField를 사용해서 프로퍼티화 하는게 좋지 않나?
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
            _player.Attack();
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


}
