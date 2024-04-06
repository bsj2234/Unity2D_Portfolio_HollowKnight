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

//    //Todo ���� �ִ�ӵ��� ����� ������ �ȵǴ� ���� �ذ�����
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

//    //Todo ���� �ִ�ӵ��� ����� ������ �ȵǴ� ���� �ذ�����
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

////�׷� ���ؽ�Ʈ�� ��𿡼� ����ؾ� �ϴ°�



//����

//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Assertions;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.LowLevel;

//public class PlayerController : MonoBehaviour
//{
//    //������ �����̳� �ٴ��̳� �ӵ� ����
//    //LaterDo:���߿� �����ϸ� �ε巴�� �ӵ��� �����ϴ¹��� �����غ��� ���߿����� ��Ʈ���� �Ը�������
//    public float PawnMaxSpeed = 5f;
//    public float PawnAccelerate = 88f;
//    public float PawnJumpPower = 500f;
//    public Animator PawnAnimator;

//    //������//Todo �ν����Ϳ��� ���̵���
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
//        //���� ��ü���� ã�� �Ⱦ ���ϵ忡�� ������Ʈ�� �ٷ� �����ͺ�
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
//    //        //Todo �÷��̾��� �Է¿� ���� addForce�� ���� ũ�� ���� �ٿ��ش�
//    //        playerRigidbody.AddForce(moveDirInput.normalized * 
//    //            Mathf.Min(PlayerAccelerate, Mathf.Max(PlayerMaxSpeed - playerRigidbody.velocity.magnitude, 0f)));
//    //    }
//    //    if(moveDirInput.magnitude < 0.1f)
//    //    {
//    //    }
//    //}

//    protected virtual void Update()
//    {
//        //���� �бⰡ ������
//        //�����ϰ� �����սô�
//        //������, �̵���, �ƹ��͵� �ȴ����� ���� ��
//        //������ �Է� ���� ���� �б�
//        //�̵��� �Է� ���� ����
//        //~�� �Է� ��������
//        //�̰� �����Ͱ���
//        //�ƴ� ���߿� ������, ���������� ,� ���������� ������ ����� ���¸� �����°� �
//        //�ٵ� Ȯ���� ���ɼ��� ���ٸ�
//        // �׷��ø�, ��Ʈ��, ���ϻ�, Ż�� ������? ������Ʈ ���� �����~~~
//        // �������� ���� ��ƴ�
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
//            //Todo ó�� ���ũ�� �����ϱ�
//            //�Է� ���⿡���� ��������Ʈ ���� ����
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
//    //�ϴ� ���ʰ��� ���°��� ������ �� ���� 

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

//    //���󰡴� �Ÿ��� �������� �ٸ� ��ų ���
//    //�Ÿ��� �����ð����� �������� �ڷ���Ʈ

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