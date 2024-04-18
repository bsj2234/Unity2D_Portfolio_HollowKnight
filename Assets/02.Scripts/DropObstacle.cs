using UnityEngine;

public class DropObstacle : MonoBehaviour
{
    public Rigidbody2D rb;

    private void Update()
    {
        if (GameManager.Instance.Player.transform.position.x - transform.position.x < 15f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 20f, LayerMask.GetMask("Character", "Ground"));
            if ((hit.transform?.CompareTag("Player")) ?? false)
            {
                rb.simulated = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager.Instance.Player.GetCombatComponent().TakeDamage(transform.position, 10f);
            gameObject.SetActive(false);
        }
    }
}
