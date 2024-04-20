using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FlipEvent : MonoBehaviour
{
    [SerializeField] private FalseKnight _owner;
    private void Awake()
    {
        _owner = _owner ?? transform.GetComponentInParent<FalseKnight>();
        Assert.IsNotNull( _owner );
    }

    public void Flip()
    {
        _owner.Flip();
    }
    public void ChangeToGroggy()
    {
        _owner.OnGroggy();
    }
}
