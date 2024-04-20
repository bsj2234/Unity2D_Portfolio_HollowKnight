using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Player _player;
    public Player Player { get => _player; }
    [SerializeField] private Camera _camera;
    [SerializeField] private bool _debugMode = true;
    public bool DebugMode { get => _debugMode; }
    private void Awake()
    {
        //this.Player = Player.FindAnyObjectByType(typeof(Player)) as Player;
        Assert.IsNotNull(this._player);
    }

    public Player GetPlayer()
    {
        return this._player;
    }

    public void CameraToPlayer()
    {
        _camera.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, _camera.transform.position.z);
    }
}
