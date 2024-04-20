using UnityEngine;

public class RespawnPoint : MonoBehaviour, IInteractable
{
    public Vector2 position;
    public GameObject InteractUi;
    private void Awake()
    {
        position = (Vector2)transform.position + Vector2.up * .5f;
    }
    public void Interact(Player player)
    {
        player.SetSpawnPoint(this);
        Debug.Log("RespawnSet");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractUi.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractUi.SetActive(false);
        }
    }
}