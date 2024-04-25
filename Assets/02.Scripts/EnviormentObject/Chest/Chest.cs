using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Chest : MonoBehaviour, IInteractable
{
    //사용하면 
    public CharmData Item;
    private Animator _animator;
    private bool _collected = false;
    [SerializeField] GameObject _aquiredUi;

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
        StartCoroutine(ShowAcquiredUi());
        _collected = true;
    }
    private IEnumerator ShowAcquiredUi()
    {
        _aquiredUi.SetActive(true);
        yield return new WaitForSeconds(1f);
        _aquiredUi.SetActive(false);
    }

}
