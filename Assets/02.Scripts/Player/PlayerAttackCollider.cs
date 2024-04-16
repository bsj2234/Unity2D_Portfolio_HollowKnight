using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerAttackCollider : MonoBehaviour
{
    Player _player;
    private Collider2D _collider;

    private void Awake()
    {
        _player = transform.GetComponentInParent<Player>();
        _collider = GetComponent<Collider2D>();
        Assert.IsNotNull( _collider );
        Assert.IsNotNull( _player );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _player.OnAttackSuccess(_collider ,collision);
    }
}
