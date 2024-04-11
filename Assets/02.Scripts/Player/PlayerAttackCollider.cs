using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerAttackCollider : MonoBehaviour
{
    Player _player;
    private void Awake()
    {
        _player = transform.GetComponentInParent<Player>();
        Assert.IsNotNull( _player );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _player.AttackKnockback(collision);
    }
}
