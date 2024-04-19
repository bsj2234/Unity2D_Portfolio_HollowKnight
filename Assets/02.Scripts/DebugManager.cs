using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _teleportPosition;

    //간편한 시스템이라 키보드만 사용
#if UNITY_EDITOR
    private void Update()
    {
        TeleportInputProcess();
    }
    #region INPUT
    private void TeleportInputProcess()
    {
        if (!Keyboard.current.shiftKey.isPressed)
            return;
        for(int i = 0; i < 12; i++)
        {
            if(Keyboard.current[Key.F1+i].isPressed == true)
            {
                Teleport(i);
                break;
            }
        }
    }

    private void Teleport(int i)
    {
        GameManager.Instance.Player.Teleport(_teleportPosition[i].transform.position);
    }
    #endregion

#endif
}
