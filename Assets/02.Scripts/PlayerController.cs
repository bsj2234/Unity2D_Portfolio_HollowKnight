using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //입력
    //지금은 공중이나 바닥이나 속도 같게
    //LaterDo:나중에 가능하면 부드럽게 속도를 제한하는법도 생각해보자 공중에서는 컨트롤이 먹먹히지게
    public float MaxSpeed = 5f;
    //public float PawnAccelerate = 88f;
    public float PawnJumpPower = 500f;
    public Vector2 moveDirInput;

    //충돌
    //디버깅용//Todo 인스펙터에만 보이도록
    //충돌검사
    [SerializeField]
    private int _collisionCounter = 0;

    //관련 컴포넌트들
    private Rigidbody2D _rigidbody;
    private MoveComponent _moveComponent;
    private SpriteRenderer _pawnSprite;
    private Animator _pawnAnimator;

    //입력관련 상태 폰의 상태?
    //프로퍼티다니까 인스펙터에 안보임
    //나중에 get이나 set에 조건이 필요할떄 프로퍼티랑 SerializeField를 사용해서 프로퍼티화 하는게 좋지 않나?
    public bool hasMoveInput = false;
    public bool isGrounded = true;
    public bool isJumping = false;
    public bool isDead = false;

    //공격 관련
    public bool isAttacking = false;

    //회피,무적
    private bool _invincible = false;
    public float RollInvincibleTime = 0.5f;

    /// <summary>이건 공격 애니메이션 이벤트에서 처리됨 </summary>
    public bool isPendingAttack = false;

    //체력
    private float hp = 100f;


    private void Awake()
    {
        //Todo 자식에 히트박스를 따로 만들고 싶다 
        //뭔가 전체에서 찾기 싫어서 차일드에서 컴포넌트를 바로 가져와봄

        //나 자신의 콜리전은 물리 검사용임
        //자식의 콜리전은 이름이있으니 패스
        //Neverminer  transform.Find하면 이름으로 찾을 수 있당
        _rigidbody = GetComponent<Rigidbody2D>();
        _moveComponent = GetComponent<MoveComponent>();
        _pawnSprite = transform.GetComponentInChildren<SpriteRenderer>();
        _pawnAnimator = transform.GetComponentInChildren<Animator>();

        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_moveComponent);
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
            //Todo 처음 상대크기 유지하기
            //입력 방향에따라 스프라이트 방향 설정
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
