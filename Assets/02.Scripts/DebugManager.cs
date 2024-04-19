using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class DebugManager : Singleton<DebugManager>
{
    [SerializeField] private GameObject[] _teleportPosition;

    //간편한 시스템이라 키보드만 사용

    float timer = 0f;
    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;
            return;
        }
        //F1~12 Check and with shift key
        //then teleport
        int functionKeyIndex = GetFunctionKey();
        if (functionKeyIndex == -1)
        {
            return;
        }
        if (Keyboard.current.shiftKey.isPressed == true)
        {
            Teleport(functionKeyIndex);
        }
        else
        {
            //f1 will heal
            //f4 will dead
            if (functionKeyIndex == 0)
                GameManager.Instance.Player.GetCombatComponent().Heal(1);
            if (functionKeyIndex == 1)
                GameManager.Instance.Player.GetCombatComponent().TakeDamage(1);
            if (functionKeyIndex == 3)
                GameManager.Instance.Player.GetCombatComponent().Die();
        }
        timer += .1f;
    }
    #region INPUT
    private int GetFunctionKey()
    {
        for (int i = 0; i < 12; i++)
        {
            if (Keyboard.current[Key.F1 + i].isPressed == true)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion

    #region FUNC
    private void Teleport(int i)
    {
        if ( i < _teleportPosition.Length)
        {
            GameManager.Instance.Player.Teleport(_teleportPosition[i].transform.position + Vector3.up * 1.5f);
        }
    }
    #endregion
}
