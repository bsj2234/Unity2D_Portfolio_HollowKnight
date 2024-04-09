using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnimationEvents : MonoBehaviour
{
    public GameObject ChestBack;

    public void ActiveChestBack()
    { ChestBack.SetActive(true); }
}
