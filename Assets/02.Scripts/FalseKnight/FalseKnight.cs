
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;

public enum FalseKnightState
{
    Idle, JumpAttack, Jump, GroundAttack, Rampage, MainBody, Dead
}
public class FalseKnight : Character, IFightable
{
    public FalseKnightState state;

    //public MoveComponent moveComponent;
    public CombatComponent combatComponent;

    public bool isGrounded;

    private Animator _animator;
    private Transform _target;
    private Rigidbody2D _rigidbody;

    //점프할때 grounded 잠시 무시해줘야 isgrounded가 예상대로 작동함 안그러면 점프 하자마자 착지했다고 판정됨
    //아니면 점프에서 isjumping set 하고 점핑과 alomostOnGroun와 Is Jumping이 둘다 false일때 착지 모션을 들어가게 하자
    private float groundIgnoreTime = 0f;
    private float _invincibleTime;
    private int Phase = 1;
    [SerializeField] Light2D _light;

    //죽었을때 나오는 소울
    [SerializeField] private FalseKnightMainBody _mainBody;
    private FalseKnightMainBody _mainBodyCombat;
    private int _mainBodyDeathCount = 0;
    public GameObject[] damagedEffects;

    public System.Action<FalseKnight> OnStatusChange;
    public System.Action OnFlip;
    public System.Action OnRealDead;

    private void Awake()
    {
        _animator = transform.GetComponentInChildren<Animator>();
        Assert.IsNotNull(_animator);
        _rigidbody = GetComponent<Rigidbody2D>();
        _target = GameManager.Instance.GetPlayer().transform;
        _mainBodyCombat = _mainBody.GetComponent<FalseKnightMainBody>();
        _mainBodyCombat.OnDead += OnMainbodyCurrentDead;
        OnRealDead += RealDead;
    }

    public void ChangeState(FalseKnightState newState)
    {
        state = newState;
        switch (newState)
        {
            case FalseKnightState.Idle:
                _animator.SetTrigger("Idle");
                break;
            case FalseKnightState.JumpAttack:
                _animator.SetTrigger("JumpAttack");
                break;
            case FalseKnightState.Jump:
                _animator.SetTrigger("Jump");
                break;
            case FalseKnightState.GroundAttack:
                _animator.SetTrigger("GroundAttack");
                break;
            case FalseKnightState.Rampage:
                _animator.SetTrigger("Rampage");
                break;
            case FalseKnightState.MainBody:
                _animator.SetTrigger("Stun");
                break;
            case FalseKnightState.Dead:
                _animator.SetTrigger("Dead");
                break;
            default:
                break;
        }
    }

    //무적 시간과 grounded 애니메이터 플래그 셋
    private void Update()
    {
        if(state == FalseKnightState.Dead)
        {
            return;
        }
        if (groundIgnoreTime > 0f)
        {
            groundIgnoreTime -= Time.deltaTime;
            _animator.SetBool("AlmostOnGround", false);
        }
        else
        {
            if (isGrounded)
                _animator.SetBool("AlmostOnGround", true);
            else
                _animator.SetBool("AlmostOnGround", false);
        }
        if (_invincibleTime > 0f) { _invincibleTime -= Time.deltaTime; }

    }


    //Ai funcs
    public void JumpToTarget(float time)
    {
        isGrounded = false;
        groundIgnoreTime = .2f;
        float distance = _target.position.x - transform.position.x;
        _rigidbody.AddForce(new Vector2(distance / time, 9.81f * time * .5f), ForceMode2D.Impulse);
        FocusToPlayer();
    }
    public void FocusToPlayer()
    {

        //Todo 이거 사실 아님 플립을 써야함 그래야 프립에서 이벤트 처리가 제대로 됨
        if (_target.position.x - transform.position.x < 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        if(OnFlip != null)
        {
            OnFlip.Invoke();
        }
    }
    public void JumpToRandom(float time)
    {
        float distance = Random.Range(-20f, 20f);
        _rigidbody.AddForce(new Vector2(distance / time, 9.81f * time * .5f), ForceMode2D.Impulse);
        FocusToPlayer();
    }

    private void OnCurrentDead()
    {
        switch (Phase)
        {
            case 1:
                _animator.SetTrigger("Groggy");
                state = FalseKnightState.MainBody;
                break;
            case 2:
                _animator.SetTrigger("Groggy");
                state = FalseKnightState.MainBody;
                break;
            case 3:
                _animator.SetTrigger("Groggy");
                state = FalseKnightState.MainBody;
                _mainBodyCombat.OnDead -= OnMainbodyCurrentDead;
                _mainBodyCombat.OnDead += OnRealDead;
                break;
        }
    }

    private void SpawnMainBody()
    {
        _mainBody.Spawn();
    }
    private void OnMainbodyCurrentDead()
    {
        _mainBodyDeathCount++;
        Phase++;
        _invincibleTime = 0f;
        combatComponent.ResetHp();
        _animator.SetTrigger("GroggyDead");
        state = FalseKnightState.Idle;
    }
    //죽으면 트리거 발생시키자 anystat로 빠진 후
    //날라가는 애니메이션 실행 메인바디 스폰 시키고
    //메인바디루프 돌다가 죽으면 다시 idle로 

    //Collisions GroundCheck
    private void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            isGrounded |= normal.y >= .9f;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        if (groundIgnoreTime > 0)
            isGrounded = false;
        if (isGrounded)
            Debug.Log(collision.gameObject.name);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        if (groundIgnoreTime > 0)
            isGrounded = false;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }


    private void DamagedEffect(Vector2 attackerPos, Vector2 damagedPos)
    {
        _light.enabled = true;
        ObjectSpawnManager.Instance.SpawnBetween(damagedEffects ,attackerPos, damagedPos, 4f);
        StartCoroutine(BlinkOff());

    }

    private IEnumerator BlinkOff()
    {
        yield return new WaitForSeconds(.2f);
        _light.enabled = false;
    }

    public string GetPhase()
    {
        return Phase.ToString();
    }
    public void Flip()
    {
        Quaternion right = Quaternion.Euler(0f, 180f, 0f);
        Quaternion left = Quaternion.Euler(0f, 0f, 0f);
        float curAngle = transform.rotation.eulerAngles.y;

        if (curAngle == 180f)
        {
            transform.rotation = left;
        }
        else
        {
            transform.rotation = right;
        }
        if (OnFlip != null)
        {
            OnFlip.Invoke();
        }
    }
    public void OnGroggy()
    {
        _invincibleTime = 99999999999999f;
        SpawnMainBody();
    }

    public float GetHp()
    {
        return combatComponent.GetHp();
    }

    public void DealFixedDamage(IFightable target, float damage)
    {
        target.TakeDamage(damage, transform.position);
    }

    public void DealDamage(IFightable target, float damage)
    {
        target.TakeDamage(damage, transform.position);
    }

    public void TakeDamage(float damage, Vector2 attackerPos)
    {
        combatComponent.TakeDamage(damage);
        if (state == FalseKnightState.MainBody)
        {
            return;
        }
        if (_invincibleTime > 0f) { return; }
        combatComponent.TakeDamage(damage);
        if (combatComponent.IsDead())
        {
            OnCurrentDead();
        }
        else
        {
            _invincibleTime = .3f;
            DamagedEffect(attackerPos, transform.position);
        }
        //UI
        if (OnStatusChange != null)
        {
            OnStatusChange.Invoke(this);
        }
        //invincible(damaged or dashing)
    }

    public void RealDead()
    {
        _animator.SetTrigger("Dead");
        state = FalseKnightState.Dead;
        ObjectSpawnManager.Instance.SpawnMoney(transform.position, 100);
        Destroy(gameObject, 5f);
    }
}
