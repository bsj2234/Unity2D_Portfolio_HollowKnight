//using UnityEngine;

//using UnityEngine.Assertions;


//public interface IContext
//{
//    void TransitionTo(IState state);
//}

//public interface IState
//{
//    public void Handle();
//}

//public abstract class PawnState : ScriptableObject ,IState
//{
//    protected PawnController pawn;
//    protected float MaxSpeed = 5f;
//    public PawnState(PawnController player) : base()
//    {
//        this.pawn = player;
//    }
//    public abstract void Handle();

//}


//public class PawnIdleState : PawnState
//{

//    //Todo 현재 최대속도를 벗어나면 감속이 안되는 현상 해결하자
//    public PawnIdleState(PawnController pawn) : base(pawn)
//    {
//        MaxSpeed = 0f;
//    }
//    public override void Handle()
//    {
//        if (pawn.grounded)
//        {
//            if (pawn.hasMoveInput)
//            {
//                pawn.TransitionTo(new PawnRunState(pawn));
//            }
//        }
//        else
//        {
//            pawn.TransitionTo(new PawnFallState(pawn));
//        }
//    }
//}

//public class PawnRunState : PawnState
//{
//    public PawnRunState(PawnController pawn) : base(pawn)
//    {
//        MaxSpeed = 6.0f;
//    }


//    public override void Handle()
//    {
//        if (pawn.grounded)
//        {
//            if (pawn.hasMoveInput)
//            {
//                pawn.MoveComponent.MoveUpdate(pawn.moveDirInput, MaxSpeed);
//            }
//        }
//        else
//        {
//            pawn.TransitionTo(new PawnFallState(pawn));
//        }
//    }
//}
//public class PawnFallState : PawnState
//{

//    //Todo 현재 최대속도를 벗어나면 감속이 안되는 현상 해결하자
//    public PawnFallState(PawnController pawn) : base(pawn)
//    {
//        MaxSpeed = 3.0f;
//    }
//    public override void Handle()
//    {
//        if (pawn.grounded)
//        {
//            pawn.TransitionTo(new PawnRunState(pawn));
//        }
//        else
//        {
//            if (pawn.hasMoveInput)
//            {
//                pawn.MoveComponent.MoveUpdate(pawn.moveDirInput, MaxSpeed);
//            }
//        }
//    }
//}

////그럼 컨텍스트를 어디에서 사용해야 하는가



//흔적

//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Assertions;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.LowLevel;

//public class PlayerController : MonoBehaviour
//{
//    //지금은 공중이나 바닥이나 속도 같게
//    //LaterDo:나중에 가능하면 부드럽게 속도를 제한하는법도 생각해보자 공중에서는 컨트롤이 먹먹히지게
//    public float PawnMaxSpeed = 5f;
//    public float PawnAccelerate = 88f;
//    public float PawnJumpPower = 500f;
//    public Animator PawnAnimator;

//    //디버깅용//Todo 인스펙터에만 보이도록
//    [SerializeField]
//    private int _collisionCounter = 0;
//    public Vector2 moveDirInput;


//    public SpriteRenderer PawnSprite;

//    public bool grounded = true;
//    public bool isJumping = false;
//    public bool hasMoveInput = false;

//    protected Rigidbody2D _rigidbody;
//    protected PawnState _pawnState;
//    public MoveComponent MoveComponent;


//    protected virtual void Awake()
//    {
//        //뭔가 전체에서 찾기 싫어서 차일드에서 컴포넌트를 바로 가져와봄
//        SpriteRenderer childSprte = transform.GetComponentInChildren<SpriteRenderer>();
//        _rigidbody = GetComponent<Rigidbody2D>();
//        MoveComponent = GetComponent<MoveComponent>();

//        Assert.IsNotNull(PawnSprite);
//        Assert.IsNotNull(PawnAnimator);
//    }


//    //private void FixedUpdate()
//    //{
//    //    if (grounded && playerRigidbody.velocity.magnitude < PlayerMaxSpeed)
//    //    {
//    //        //Todo 플레이어의 입력에 의해 addForce할 값이 크면 적게 줄여준다
//    //        playerRigidbody.AddForce(moveDirInput.normalized * 
//    //            Mathf.Min(PlayerAccelerate, Mathf.Max(PlayerMaxSpeed - playerRigidbody.velocity.magnitude, 0f)));
//    //    }
//    //    if(moveDirInput.magnitude < 0.1f)
//    //    {
//    //    }
//    //}

//    protected virtual void Update()
//    {
//        //현재 분기가 복잡함
//        //간단하게 생각합시다
//        //점프시, 이동시, 아무것도 안누르고 있을 시
//        //점프시 입력 있음 없음 분기
//        //이동시 입력 있음 없음
//        //~시 입력 있음없음
//        //이게 좋을것같다
//        //아니 공중에 있을시, 땅에있을시 ,어떤 진흙지형에 있을시 등등의 상태를 가지는건 어떄
//        //근데 확장할 가능성이 높다면
//        // 그래플링, 제트팩, 낙하산, 탈것 많은데? 스테이트 패턴 써야지~~~
//        // 생각보다 많이 어렵다
//        //
//        _pawnState.Handle();

//    }

//    public Vector2 GetPlayerVelocity()
//    {
//        return _rigidbody.velocity;
//    }

//    public void SetDrag(float drag)
//    {
//        _rigidbody.drag = drag;
//    }

//    protected void Attack()
//    {
//        //SetPlayerAttackState
//    }

//    //OnInput
//    //Only Set InputVector
//    //Input Handled In PlayerMoveState
//    public void OnMove(InputValue inputValue)
//    {
//        float xInput = inputValue.Get<Vector2>().x;
//        hasMoveInput = Mathf.Abs(xInput) > .1f;
//        if (hasMoveInput)
//        {
//            PawnAnimator.SetBool("Run", true);
//            moveDirInput = new Vector2(xInput, 0f);
//            //Todo 처음 상대크기 유지하기
//            //입력 방향에따라 스프라이트 방향 설정
//            PawnSprite.flipX = (xInput < 0f) ? true : false;
//        }
//        else
//        {
//            PawnAnimator.SetBool("Run", false);
//            moveDirInput = Vector2.zero;
//        }
//    }
//    protected void OnJump(InputValue inputValue)
//    {
//        if (inputValue.isPressed == true)
//        {
//            _rigidbody.AddForce(PawnJumpPower * Vector2.up);
//            isJumping = true;
//            PawnAnimator.SetBool("Grounded", false);
//            PawnAnimator.SetTrigger("Jump");
//        }
//    }
//    protected void OnAttack(InputValue inputValue)
//    {
//        if (inputValue.isPressed == false)
//        {
//            Attack();
//        }
//    }
//    //Collisions
//    protected void OnCollisionEnter2D(Collision2D collision)
//    {
//        _collisionCounter++;
//        grounded = true;
//        PawnAnimator.SetBool("Grounded", true);
//        isJumping = false;
//    }
//    protected void OnCollisionExit2D(Collision2D collision)
//    {
//        _collisionCounter--;
//        if (_collisionCounter == 0)
//        {
//            grounded = false;
//        }
//        if (!isJumping && !grounded)
//        {
//            PawnAnimator.SetBool("Grounded", false);
//        }
//    }

//    //IContext
//    public void TransitionTo(IState state)
//    {
//        if (state is PawnState ps)
//        {
//            this._pawnState = ps;
//        }
//        else
//        {
//            Assert.IsFalse(true, "not a pawn state");
//        }

//    }
//}


//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Assertions;
//using static UnityEngine.RuleTile.TilingRuleOutput;





//enum EbossKnightState
//{
//    Idle,
//    Follow,
//    TeleportAttack,
//    JumpAttack,
//    BackJump,
//}



//public abstract class BossKnightState : IState
//{
//    protected BossKnightController controller;
//    protected float counter = 0f;
//    protected float timer = 2f;
//    public abstract void Handle();
//}

//public class BossKnightIdleState : BossKnightState
//{
//    //일단 몇초간은 쉬는것을 실행한 후 변경 

//    public BossKnightIdleState(float IdleTime)
//    {
//        timer = IdleTime;
//    }

//    public override void Handle()
//    {
//        counter += Time.deltaTime;
//        if (counter >= timer)
//        {
//            controller.TransitionTo(controller.GetRandomState());
//        }
//    }
//}



//class BossKnightFollowState : BossKnightState
//{
//    public BossKnightFollowState(float followTime)
//    {
//        timer = followTime;
//    }

//    //따라가다 거리가 좁아지면 다른 스킬 사용
//    //거리를 일정시간동안 못좁히면 텔레포트

//    public override void Handle()
//    {
//        counter += Time.deltaTime;
//        if (counter >= timer)
//        {
//            //if(IsInAttackDistanse())
//            {

//            }
//        }
//    }
//}






//public class BossKnightController : PawnController
//{

//    bool fighting = false;
//    public Transform target;

//    private void Update()
//    {
//        base.Update();
//    }


//    private void RandomFightSkill()
//    {

//        int a = Random.Range(0, 5);
//        switch (a)
//        {
//            case 0:
//                Follow();
//                break;
//            default:
//                break;
//        }
//    }

//    void Follow()
//    {
//        Vector2 toTarget = target.position - transform.position;

//        float xDir = toTarget.x;
//        xDir = Mathf.Sign(xDir);
//        moveDirInput = new Vector2(xDir, 0f);

//    }

//    void Teleport()
//    {

//    }

//    public BossKnightState GetRandomState()
//    {
//        return null;
//    }
//}