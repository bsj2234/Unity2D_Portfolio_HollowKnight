using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Chest : MonoBehaviour, IInteractable
{
    //����ϸ� 
    public CharmData Item;
    private Animator _animator;
    private bool _collected = false;

    private void Awake()
    {
        _animator = transform.GetComponentInChildren<Animator>();
        Assert.IsNotNull( _animator );
    }
    public void Interact(Player player)
    {
        if(_collected) return;
        player.AddItem(new CharmInstance(Item));
        _animator.SetTrigger("Open");
        _collected = true;
    }


}
