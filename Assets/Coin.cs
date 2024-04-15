using UnityEngine;

public class Coin : MonoBehaviour
{
    public Rigidbody2D rb;
    private Player player;

    public void OnEnable()
    {
        Vector2 dir = new Vector2(Random.Range(-1f, 1f), 1f);
        dir = dir.normalized;
        rb.AddForce(dir * Random.Range(5f, 10f), ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //use object pool manager to cancle coroutine and return coin and aad coin to player
            ObjectSpawnManager.Instance.ReturnMoney(transform.parent.gameObject);
            if (player == null)
            {
                player = collision.transform.GetComponent<Player>();
            }
            GameManager.Instance.GetPlayer().AddCoin(1);

        }
    }
}
