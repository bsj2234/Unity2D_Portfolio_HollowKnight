using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _teleportPosition;

    //������ �ý����̶� Ű���常 ���
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
