using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Animator _anim;
    private Player player;
    private bool _isGrounded;

    public void OnEnable()
    {
        Vector2 dir = new Vector2(Random.Range(-1f, 1f), 1f);
        dir = dir.normalized;
        _rb.AddForce(dir * Random.Range(5f, 10f), ForceMode2D.Impulse);
    }

    private void LateUpdate()
    {

        _anim.SetBool("IsOnGround", _isGrounded);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _anim.SetTrigger("Acquired");
            //use object pool manager to cancle coroutine and return coin and aad coin to player
            PooledObjectSpawnManager.Instance.ReturnMoney(transform.parent.gameObject, 1f);
            if (player == null)
            {
                player = collision.transform.GetComponent<Player>();
            }
            GameManager.Instance.GetPlayer().AddCoin(1);

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //groundCheck
        bool groundChecker = false;
        foreach(var contact in collision.contacts)
        {
            groundChecker |= contact.normal.y > .7f;
        }
        if (groundChecker)
            _isGrounded = true;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {

        //groundCheck
        bool groundChecker = false;
        foreach (var contact in collision.contacts)
        {
            groundChecker |= contact.normal.y > .7f;
        }
        if (groundChecker)
            _isGrounded = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {

        //groundCheck
        bool groundChecker = false;
        foreach (var contact in collision.contacts)
        {
            groundChecker |= contact.normal.y > .7f;
        }
        if (groundChecker)
            _isGrounded = true;
    }

}
