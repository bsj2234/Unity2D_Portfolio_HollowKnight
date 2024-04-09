using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : Singleton<GameManager>
{
    public Player Player;

    private void Awake()
    {
        //this.Player = Player.FindAnyObjectByType(typeof(Player)) as Player;
        Assert.IsNotNull(this.Player);
    }

    public Player GetPlayer()
    {
        return this.Player;
    }
}
