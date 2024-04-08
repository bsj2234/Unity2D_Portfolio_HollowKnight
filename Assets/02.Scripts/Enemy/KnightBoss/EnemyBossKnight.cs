using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class EnemyBossKnight : MonoBehaviour
{
    private EnemyBossKnightController _controller;
    private EnemyBossKnightMoveComponent _moveComponent;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _pawnSprite;
    private Animator _pawnAnimator;

    //������
    public bool isJumping = false;
    public bool isDead = false;
    //���� ����
    /// <summary>�̰� ���� �ִϸ��̼� �̺�Ʈ���� ó���� </summary>
    public bool isPendingAttack = false;
    public bool isAttacking = false;
    //ȸ��,����
    private bool _invincible = false;
    public float RollInvincibleTime = 0.5f;

    //ü��
    private float hp = 100f;

    //���ݿ� ��ġ
    public Transform[] WallSides = new Transform[2];
    public Transform BattleZone;

    //�ִϸ��̼�
    public List<Rigidbody2D> _animationColliders = new List<Rigidbody2D>();

    //���ݿ�
    public Transform Target;
    void Awake()
    {

        //Todo �ڽĿ� ��Ʈ�ڽ��� ���� ����� �ʹ� 
        //���� ��ü���� ã�� �Ⱦ ���ϵ忡�� ������Ʈ�� �ٷ� �����ͺ�

        //�� �ڽ��� �ݸ����� ���� �˻����
        //�ڽ��� �ݸ����� �̸��������� �н�
        //Neverminer  transform.Find�ϸ� �̸����� ã�� �� �ִ�
        _controller = GetComponent<EnemyBossKnightController>();
        _moveComponent = GetComponent<EnemyBossKnightMoveComponent>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _pawnSprite = transform.GetComponentInChildren<SpriteRenderer>();
        _pawnAnimator = transform.GetComponentInChildren<Animator>();
        Assert.IsNotNull( _controller );
        Assert.IsNotNull( _moveComponent );
        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_pawnSprite);
        Assert.IsNotNull(_pawnAnimator);
        Assert.IsNotNull(BattleZone);
        Assert.IsNotNull(WallSides[0]);
        Assert.IsNotNull(WallSides[1]);
        Assert.IsNotNull(Target);


        Assert.IsTrue(_animationColliders.Count > 0, "SetAnimationColliders");

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
        ////�Է� ���⿡���� ��������Ʈ ���� ����
        //if (_moveComponent.dir == Direction.Left)
        //{
        //    _pawnSprite.flipX = true;
        //}
        //else
        //{
        //    _pawnSprite.flipX = false;
        //}

        if(_controller.hasMoveInput)
        {
            _pawnAnimator.SetBool("Anim_IsRunning", true);
        }
        else
        {
            _pawnAnimator.SetBool("Anim_IsRunning", false);
        }

        if (_moveComponent.isGrounded)
        {
            _pawnAnimator.SetBool("Anim_IsGrounded", true);
            isJumping = false;
        }
        else
        {
            _pawnAnimator.SetBool("Anim_IsGrounded", false);
        }
    }


    public Vector2 GetPlayerVelocity()
    {
        return _rigidbody.velocity;
    }

    public void SetDrag(float drag)
    {
        _rigidbody.drag = drag;
    }



    public void Attack()
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

        if(_moveComponent.isGrounded)
        {
            _rigidbody.velocity = Vector2.zero;
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Damaged(10f);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
    }

    public void Damaged(float damage)
    {
        hp -= damage;
        if (hp < 0f)
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

    public void StartJump()
    {
        _moveComponent.StartJump();
        isJumping = true;
        _pawnAnimator.SetBool("Anim_IsGrounded", false);
        _pawnAnimator.SetTrigger("Anim_Jump");
    }
    public void EndJump()
    {
        _moveComponent.EndJump();
    }

    public void Dodge()
    {
        _pawnAnimator.SetTrigger("Anim_Dodge");
        _invincible = true;
        StartCoroutine(InvinsibleOff());
    }

    public void Move(Vector2 input)
    {
        _moveComponent.Move(input);
    }

    private void JumpToTarget(Vector2 target, float time)
    {
        float distance = target.x - transform.position.x;
        _rigidbody.AddForce(new Vector2(distance / time  ,9.81f * time * .5f), ForceMode2D.Impulse);
    }

    public void JumpToNearWallSide(float timeToTarget)
    {
        JumpToTarget(GetFarWall().position, timeToTarget);
    }

    private Transform GetNearWall()
    {
        if (transform.position.x < BattleZone.position.x)
        {
            return WallSides[0];
        }
        else
        {
            return WallSides[1];
        }
    }
    private Transform GetFarWall()
    {
        if (transform.position.x > BattleZone.position.x)
        {
            return WallSides[0];
        }
        else
        {
            return WallSides[1];
        }
    }
    //aniamtion collider on and off
    public void ActiveCollider(string colliderName)
    {
        _animationColliders.Find(x => x.name == colliderName).simulated=true;
    }
    public void DeactiveCollider(string colliderName)
    {
        _animationColliders.Find(x => x.name == colliderName).simulated = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerAttackCollider"))
        {
            Debug.Log(collision.gameObject.name);
            _pawnAnimator.SetTrigger("Anim_Damaged");
            hp -= 40f;
            if (hp < 0)
            {
                OnDead();
            }
        }
    }

    private void OnDead()
    {
        _pawnAnimator.SetTrigger("Anim_Dead");
    }

    public void FocusToPlayer()
    {
        if(Target.position.x - transform.position.x < 0f)
        {
            _pawnSprite.flipX = true;
        }
        else
        {
            _pawnSprite.flipX = false;
        }
    }
}
