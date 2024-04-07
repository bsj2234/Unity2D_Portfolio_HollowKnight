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

    //폰상태
    public bool isJumping = false;
    public bool isDead = false;
    //공격 관련
    /// <summary>이건 공격 애니메이션 이벤트에서 처리됨 </summary>
    public bool isPendingAttack = false;
    public bool isAttacking = false;
    //회피,무적
    private bool _invincible = false;
    public float RollInvincibleTime = 0.5f;

    //체력
    private float hp = 100f;

    // Start is called before the first frame update
    void Awake()
    {

        //Todo 자식에 히트박스를 따로 만들고 싶다 
        //뭔가 전체에서 찾기 싫어서 차일드에서 컴포넌트를 바로 가져와봄

        //나 자신의 콜리전은 물리 검사용임
        //자식의 콜리전은 이름이있으니 패스
        //Neverminer  transform.Find하면 이름으로 찾을 수 있당
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
    }


    //너무 움직임이 무거워서 탈락  impulse 썻으면 됐을수도? 그래도 velocity가 최대값 관리하기 좋은듯
    //private void FixedUpdate()
    //{
    //    if (grounded && playerRigidbody.velocity.magnitude < PlayerMaxSpeed)
    //    {
    //        //Todo 플레이어의 입력에 의해 addForce할 값이 크면 적게 줄여준다
    //        playerRigidbody.AddForce(moveDirInput.normalized * 
    //            Mathf.Min(PlayerAccelerate, Mathf.Max(PlayerMaxSpeed - playerRigidbody.velocity.magnitude, 0f)));
    //    }
    //    if(moveDirInput.magnitude < 0.1f)
    //    {
    //    }
    //}
    private void Update()
    {
        //입력 방향에따라 스프라이트 방향 설정
        if (_moveComponent.dir == Direction.Left)
        {
            _pawnSprite.flipX = true;
        }
        else
        {
            _pawnSprite.flipX = false;
        }

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
        //첫공격은 트리거로
        //두번쨰부터는 pending check로

        //복잡ㅈ.. 말로풀자
        //만약 공격하고 있으면 다음공격준비
        //만약 공격중인데 다음공격준비도 되어있으면 아무것도안함
        //만약 공격중이 아니면 공격 트리거
        //만약 공격중이 아니고 다음공격이준비되어있으면 다음 공격 호출
        //그럼 매 프레임마다 다음 공격이 있는지 확인해야겠네 흠흠
        //아니 공격 애니메이션이 끝났을때 이어서 할지 정하면 되지


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
        //나머지 애니메이션 끝날떄 초기화는 AllSlash 애니메이션 EventOnAttackInterrupted 스크립트에 OnStateExit 참고

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

}
