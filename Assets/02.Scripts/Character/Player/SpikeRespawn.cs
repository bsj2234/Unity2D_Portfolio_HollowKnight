using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeRespawn : MonoBehaviour
{
    public Vector2 position;
    private void Awake()
    {
        SetPos();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        GameManager.Instance.Player.SetSpikeRespawnPoint(this);
    }

    private void SetPos()
    {
        position = (Vector2)transform.position + Vector2.up * .5f;
    }
}
