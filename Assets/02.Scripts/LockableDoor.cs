using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockableDoor : MonoBehaviour
{
    //Trigger will reference this gameobject
    //when triggerd door will shut
    // and if all target has acomplished door will open
    //zone will manage all of them

    // how the zone kmows target is dead? 
    //need event On Dead;

    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Close()
    {
        _animator.SetTrigger("Close");
    }
    public void Open()
    {
        _animator.SetTrigger("Open");
    }
}
