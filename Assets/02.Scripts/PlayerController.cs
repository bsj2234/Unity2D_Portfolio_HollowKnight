using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //�Է�
    //������ �����̳� �ٴ��̳� �ӵ� ����
    //LaterDo:���߿� �����ϸ� �ε巴�� �ӵ��� �����ϴ¹��� �����غ��� ���߿����� ��Ʈ���� �Ը�������
    public float MaxSpeed = 5f;
    //public float PawnAccelerate = 88f;
    public float PawnJumpPower = 500f;
    public Vector2 moveDirInput;

    //�浹
    //������//Todo �ν����Ϳ��� ���̵���
    //�浹�˻�
    [SerializeField]
    private int _collisionCounter = 0;

    //���� ������Ʈ��
    private Rigidbody2D _rigidbody;
    private MoveComponent _moveComponent;
    private SpriteRenderer _pawnSprite;
    private Animator _pawnAnimator;

    //�Է°��� ���� ���� ����?
    //������Ƽ�ٴϱ� �ν����Ϳ� �Ⱥ���
    //���߿� get�̳� set�� ������ �ʿ��ҋ� ������Ƽ�� SerializeField�� ����ؼ� ������Ƽȭ �ϴ°� ���� �ʳ�?
    public bool hasMoveInput = false;
    public bool isGrounded = true;
    public bool isJumping = false;
    public bool isDead = false;

    //���� ����
    public bool isAttacking = false;

    //ȸ��,����
    private bool _invincible = false;
    public float RollInvincibleTime = 0.5f;

    /// <summary>�̰� ���� �ִϸ��̼� �̺�Ʈ���� ó���� </summary>
    public bool isPendingAttack = false;

    //ü��
    private float hp = 100f;


    private void Awake()
    {
        //Todo �ڽĿ� ��Ʈ�ڽ��� ���� ����� �ʹ� 
        //���� ��ü���� ã�� �Ⱦ ���ϵ忡�� ������Ʈ�� �ٷ� �����ͺ�

        //�� �ڽ��� �ݸ����� ���� �˻����
        //�ڽ��� �ݸ����� �̸��������� �н�
        //Neverminer  transform.Find�ϸ� �̸����� ã�� �� �ִ�
        _rigidbody = GetComponent<Rigidbody2D>();
        _moveComponent = GetComponent<MoveComponent>();
        _pawnSprite = transform.GetComponentInChildren<SpriteRenderer>();
        _pawnAnimator = transform.GetComponentInChildren<Animator>();

        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_moveComponent);
        Assert.IsNotNull(_pawnSprite);
        Assert.IsNotNull(_pawnAnimator);
    }

    //�ʹ� �������� ���ſ��� Ż��  impulse ������ ��������? �׷��� velocity�� �ִ밪 �����ϱ� ������
    //private void FixedUpdate()
    //{
    //    if (grounded && playerRigidbody.velocity.magnitude < PlayerMaxSpeed)
    //    {
    //        //Todo �÷��̾��� �Է¿� ���� addForce�� ���� ũ�� ���� �ٿ��ش�
    //        playerRigidbody.AddForce(moveDirInput.normalized * 
    //            Mathf.Min(PlayerAccelerate, Mathf.Max(PlayerMaxSpeed - playerRigidbody.velocity.magnitude, 0f)));
    //    }
    //    if(moveDirInput.magnitude < 0.1f)
    //    {
    //    }
    //}

    private void Update()
    {
        _moveComponent.MoveUpdate(moveDirInput, MaxSpeed);
    }

    public Vector2 GetPlayerVelocity()
    {
        return _rigidbody.velocity;
    }

    public void SetDrag(float drag)
    {
        _rigidbody.drag = drag;
    }


    //OnInput
    //Only Set InputVector
    //Input Handled In PlayerMoveState
    private void OnMove(InputValue inputValue)
    {
        float xInput = inputValue.Get<Vector2>().x;
        hasMoveInput = Mathf.Abs(xInput) > .1f;
        if (hasMoveInput)
        {
            _pawnAnimator.SetBool("Anim_IsRunning", true);
            moveDirInput = new Vector2(xInput, 0f);
            //Todo ó�� ���ũ�� �����ϱ�
            //�Է� ���⿡���� ��������Ʈ ���� ����
            _pawnSprite.flipX = (xInput < 0f) ? true : false;
        }
        else
        {
            _pawnAnimator.SetBool("Anim_IsRunning", false);
            moveDirInput = Vector2.zero;
        }
    }
    private void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed == true)
        {
            _rigidbody.AddForce(PawnJumpPower * Vector2.up);
            isJumping = true;
            isGrounded = false;
            _pawnAnimator.SetBool("Anim_IsGrounded", false);
            _pawnAnimator.SetTrigger("Anim_Jump");
        }
    }
    private void OnAttack(InputValue inputValue)
    {
        if (inputValue.isPressed == false)
        {
            Attack();
        }
    }
    private void Attack()
    {
        //ù������ Ʈ���ŷ�
        //�ι������ʹ� pending check��

        //���⤸.. ����Ǯ��
        //���� �����ϰ� ������ ���������غ�
        //���� �������ε� ���������غ� �Ǿ������� �ƹ��͵�����
        //���� �������� �ƴϸ� ���� Ʈ����
        //���� �������� �ƴϰ� �����������غ�Ǿ������� ���� ���� ȣ��
        //�׷� �� �����Ӹ��� ���� ������ �ִ��� Ȯ���ؾ߰ڳ� ����
        //�ƴ� ���� �ִϸ��̼��� �������� �̾ ���� ���ϸ� ����


        if (!isAttacking)
        {
            _pawnAnimator.SetTrigger("Anim_Attack_Slash");
            _pawnAnimator.SetBool("Anim_IsAttacking_Slash", true);
            isAttacking = true;
        }
        else
        {
            if (!isPendingAttack)
            {
                _pawnAnimator.SetBool("Anim_IsAttacking_Slash", true);
                isPendingAttack = true;
            }
        }
        //������ �ִϸ��̼� ������ �ʱ�ȭ�� AllSlash �ִϸ��̼� EventOnAttackInterrupted ��ũ��Ʈ�� OnStateExit ����

    }
    private void OnDodge(InputValue inputValue)
    {
        if (inputValue.isPressed == false)
        {
            _pawnAnimator.SetTrigger("Anim_Dodge");
            _invincible = true;
            StartCoroutine(InvinsibleOff());
        }
    }

    private IEnumerator InvinsibleOff()
    {
        yield return new WaitForSeconds(RollInvincibleTime);
        _invincible = false;
    }

    //Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Damaged(10f);
        }    
        _collisionCounter++;
        isGrounded = true;
        _pawnAnimator.SetBool("Anim_IsGrounded", true);
        isJumping = false;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _collisionCounter--;
        if (_collisionCounter == 0)
        {
            isGrounded = false;
            _pawnAnimator.SetBool("Anim_IsGrounded", false);

        }
    }

    public void Damaged(float damage)
    {
        hp -= damage;
        if(hp < 0f)
        {
            _pawnAnimator.SetTrigger("Anim_Dead");
            _pawnAnimator.SetBool("Anim_IsDead", true);
            isDead = true;
        }
        else
        {
            _pawnAnimator.SetTrigger("Anim_Damaged");
        }
    }
    
}
