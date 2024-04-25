using System.Collections;
using UnityEngine;

public class RespawnPoint : MonoBehaviour, IInteractable
{
    public Vector2 position;
    public GameObject InteractUi;
    [SerializeField]private GameObject _SetRespawnPointUi;

    private void Awake()
    {
        position = (Vector2)transform.position + Vector2.up * .5f;
    }
    public void Interact(Player player)
    {
        player.SetSpawnPoint(this);
        StartCoroutine(ShowAcquiredUi());
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


    private IEnumerator ShowAcquiredUi()
    {
        _SetRespawnPointUi.SetActive(true);
        yield return new WaitForSeconds(1f);
        _SetRespawnPointUi.SetActive(false);
    }
}