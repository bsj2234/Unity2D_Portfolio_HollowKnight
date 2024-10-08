
using UnityEngine;

public class BattleZone : MonoBehaviour
{
    public LockableDoor[] Doors;

    public Spawner TargetSpawner;

    public EnemySpawnTrigger battleSpawnZone;

    public int enemyCount = 1;

    public int DeathCount = 0;

    public bool Locked = false;

    public bool Cleared { get; private set; } = false;

    private void Start()
    {
        GameManager.Instance.Player.OnPlayerRespawn += ResetZone;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Cleared)
            return;
        if(collision.CompareTag("Player") && !Locked)
        { 
            FalseKnight falseKnight = TargetSpawner._intancedEnemy.GetComponent<FalseKnight>();
            falseKnight.OnRealDead += AddDeath;
            foreach (LockableDoor door in Doors)
            {
                door.Close();
            }
            Locked = true;
        }
    }
    private void OpenDoors()
    {
        foreach (LockableDoor door in Doors)
        {
            door.Open();
        }
        Locked = false;
    }
    private void AddDeath()
    {
        DeathCount++;
        if(DeathCount >= enemyCount)
        {
            Cleared = true;
            OpenDoors();
            battleSpawnZone.IsSpawnable = false;
        }
    }
    private void ResetZone()
    {
        OpenDoors();
        battleSpawnZone.ResetTrigger();
        TargetSpawner.DeSpawn();
        DeathCount = 0;
    }
}
