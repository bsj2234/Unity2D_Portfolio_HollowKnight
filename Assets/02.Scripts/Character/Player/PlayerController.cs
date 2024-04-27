using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveDirInput;

    //관련 컴포넌트들
    private Player _player;
    public InventoryUi _inventoryUi;
    //입력관련 상태
    //프로퍼티다니까 인스펙터에 안보임
    //나중에 get이나 set에 조건이 필요할떄 프로퍼티랑 SerializeField를 사용해서 프로퍼티화 하는게 좋지 않나?
    public bool hasMoveInput = false;

    public bool _playerControlable = true;




    private void Awake()
    {
        _player = GetComponent<Player>();

        Assert.IsNotNull(_player);

        Assert.IsNotNull(_inventoryUi);
    }


    private void FixedUpdate()
    {
        if (_playerControlable == false)
        { return; }
        _player.Move(moveDirInput);
    }

    public void ResetControl()
    {
        hasMoveInput = false;
    }

    //OnInput
    private void OnMove(InputValue inputValue)
    {
        if(GameManager.Instance.Player.GetCombatComponent().IsDead()) { return; }
        if (_playerControlable == false)
        { return; }
        moveDirInput = inputValue.Get<Vector2>();
        hasMoveInput = Mathf.Abs(moveDirInput.x) > .1f;
    }
    private void OnJump(InputValue inputValue)
    {
         if (GameManager.Instance.Player.GetCombatComponent().IsDead()) { return; }
        if (_playerControlable == false)
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
        if (GameManager.Instance.Player.GetCombatComponent().IsDead()) { return; }
        if (_playerControlable == false)
        { return; }
        if (inputValue.isPressed == false)
        {
            _player.Attack(GetAttackDir());
        }
    }
    private void OnDodge(InputValue inputValue)
    {
        if (GameManager.Instance.Player.GetCombatComponent().IsDead()) { return; }
        if (_playerControlable == false)
        { return; }
        if (inputValue.isPressed == true)
        {
            _player.Dodge();
        }
    }

    private void OnInteract(InputValue inputValue)
    {
        if (GameManager.Instance.Player.GetCombatComponent().IsDead()) { return; }
        if (_playerControlable == false)
        { return; }
        if(inputValue.isPressed != false)
        {
            _player.TryInteract();
        }
    }    

    private void OnInventory(InputValue inputValue)
    {
        if (GameManager.Instance.Player.GetCombatComponent().IsDead()) { return; }
        if (inputValue.isPressed != false)
        {
            if(_playerControlable)
            {
                _playerControlable = false;
                UiManager.Instance.InventoryOn();
            }
        }
    }

    private void OnEscape(InputValue inputValue)
    {
        if(inputValue.isPressed != false)
        {
            if(UiManager.Instance.IsInUiMode)
            {
                UiManager.Instance.ExitUiMode();
                _playerControlable = true;
            }
            else
            {
                UiManager.Instance.PauseOn();
                _playerControlable = false;
            }
        }
    }

    public Vector2 GetAttackDir()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 playerToMouse = -(Vector2)_player.transform.position + mousePosition;
        playerToMouse = playerToMouse.normalized;

        //공격방향 설정
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

        ////공격방향 설정
        //if (playerToMouse.x > .7f)
        //{
        //    return Vector2.right;
        //}
        //if (playerToMouse.x < -.7f)
        //{
        //    return Vector2.left;
        //}
        //if (playerToMouse.y > .7f)
        //{
        //    return Vector2.up;
        //}
        //if (playerToMouse.y < -.7f)
        //{
        //    return Vector2.down;
        //}
        //Assert.IsTrue(false, "can not find mouse dir vector");
        return playerToMouse;
    }
}
