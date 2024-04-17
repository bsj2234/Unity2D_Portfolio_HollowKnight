using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : Character, IFightable
{
    private PlayerController _controller;
    private Rigidbody2D _rigidbody;
    private Transform _pawnSprite;
    private Animator _pawnAnimator;

    public HudUi hud;

    public PlayerMoveComponent moveComponent;

    //폰상태
    public bool isJumping = false;
    public bool isDead = false;
    //공격 관련
    /// <summary>이건 공격 애니메이션 이벤트에서 처리됨 </summary>
    public bool isPendingAttack = false;
    public bool isAttacking = false;
    private Vector2 _attackDir;
    private float _attackingTime = 0f;
    private int continuableAttackCount = 0;

    //회피,무적
    private float _invincibleTime = 0f;
    private float _stunTime = 0f;
    public float DodgeInvincibleTime = 0.5f;

    //체력
    public float hp = 4f;
    public int initialMaxHp = 4;
    public float maxHp = 4f;
    public float mp = 0f;
    public float maxMp = 100f;
    //뒤집기용
    Vector2 curLocScale;

    //아이템
    private float item_Damage = 0f;
    private float item_attackSpeed = 0f;
    private float item_hitInvincible = 0f;

    //인벤토리 //Todo: 인벤 디버그용 public, 참 정리하기
    private CharmInstance[] _equippedCharms = new CharmInstance[5];
    public CharmData debugCharm;
    // Wow
    //public으로 지정하면 null로 초기화되지않고 기본 생성자가 호출되는듯??
    //근데 기본생성자가 없을텐데
    //기본생성자 테스트
    //ItemInstance test = new ItemInstance();//기본생성자 생성 안되는데 인수를 null로 주나?
    //ItemInstance test2 = new ItemInstance(null);// 아마 이렇게 가지 않았을까
    private CharmInstance[] _charmInventory = new CharmInstance[24];

    private HashSet<string> _currentCharmEffects = new HashSet<string>();
    public float damagePerSlot = 30f;
    public int coinCount = 0;

    public ShopUi shopUi;
    private float _knockBackTime = 0f;

    //Respawn
    public RespawnPoint _respawnPoint;

    public GameObject HitEffect;
    public GameObject DeadEffect;

    public SpikeRespawn _spikeRespawn;

    // Start is called before the first frame update
    void Awake()
    {
        //Todo 자식에 히트박스를 따로 만들고 싶다 
        //뭔가 전체에서 찾기 싫어서 차일드에서 컴포넌트를 바로 가져와봄

        //나 자신의 콜리전은 물리 검사용임
        //자식의 콜리전은 이름이있으니 패스
        //Neverminer  transform.Find하면 이름으로 찾을 수 있당
        /*
         * 사용예
         * 찾기 쉽게 하기 위한 방법을 찾아볼까
         * Enum쓸래?
         * List[Enum] = true,false;
         * if(LIst[CharmName])
         * {
         *      DoSomething
         * }
         */
        _controller = GetComponent<PlayerController>();
        moveComponent = GetComponent<PlayerMoveComponent>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _pawnSprite = transform.GetComponentInChildren<Transform>();
        _pawnAnimator = transform.GetComponentInChildren<Animator>();
        curLocScale = _pawnSprite.transform.localScale;
        Assert.IsNotNull(_controller);
        Assert.IsNotNull(moveComponent);
        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_pawnSprite);
        Assert.IsNotNull(_pawnAnimator);

        _equippedCharms[0] = new CharmInstance(debugCharm);
    }

    private void Start()
    {
        hud.RefreshAll();
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
        if(isDead) return;
        if (_controller.hasMoveInput)
        {
            _pawnAnimator.SetBool("Anim_IsRunning", true);
            if (moveComponent.dir == Direction.Left)
            {
                _pawnSprite.transform.localScale = new Vector2(-curLocScale.x, curLocScale.y);
            }
            else
            {
                _pawnSprite.transform.localScale = new Vector2(curLocScale.x, curLocScale.y);
            }
        }
        else
        {
            //입력없으면 보는곳으로 회전
            _pawnAnimator.SetBool("Anim_IsRunning", false);
            if (_controller.GetLookDir().x <= 0f)
            {
                _pawnSprite.transform.localScale = new Vector2(-curLocScale.x, curLocScale.y);
            }
            else
            {
                _pawnSprite.transform.localScale = new Vector2(curLocScale.x, curLocScale.y);
            }
        }

        if (moveComponent.isGrounded)
        {
            _pawnAnimator.SetBool("Anim_IsGrounded", true);
            isJumping = false;
        }
        else
        {
            _pawnAnimator.SetBool("Anim_IsGrounded", false);
        }
        //상태 지속 시간을 코드에서 관리
        if (_invincibleTime > 0f) { _invincibleTime -= Time.deltaTime; }
        if (_stunTime > 0f)
        {
            _stunTime -= Time.deltaTime;
            _pawnAnimator.SetBool("Anim_IsStun", true);
        }
        else
        {
            _pawnAnimator.SetBool("Anim_IsStun", false);
        }
        if (_attackingTime > 0f) { _attackingTime -= Time.deltaTime; }
        else { continuableAttackCount = 0; }
        if (_knockBackTime > 0f) { _knockBackTime -= Time.deltaTime; }
    }
    public void Attack(Vector2 attackDir)
    {
        if (isDead) return;
        if (IsStuned() | IsAttacking())
        {
            return;
        }
        _attackingTime = .3f - item_attackSpeed;
        if (attackDir.y > .7f)
        {
            continuableAttackCount = 0;
            _pawnAnimator.SetTrigger("Anim_Attack_Slash_Up");
            continuableAttackCount++;
        }
        else if (attackDir.y < -.7f)
        {
            continuableAttackCount = 0;
            _pawnAnimator.SetTrigger("Anim_Attack_Slash_Down");
        }
        else
        {
            if (continuableAttackCount % 2 == 0)
            {
                _pawnAnimator.SetTrigger("Anim_Attack_Slash");
            }
            continuableAttackCount++;
        }
        //다음공격 대기를 애니메이션 이벤트로 공격이끝날떄 체크하고 attaking 값 설정해줌
        //나머지 애니메이션 끝날떄 초기화는 AllSlash 애니메이션 EventOnAttackInterrupted 스크립트에 OnStateExit 참고

    }

    private bool IsAttacking()
    {
        return (_attackingTime > 0f);
    }

    public void StartJump()
    {
        if (isDead) return;
        moveComponent.StartJump();
        isJumping = true;
        _pawnAnimator.SetBool("Anim_IsGrounded", false);
        _pawnAnimator.SetTrigger("Anim_Jump");
    }
    public void EndJump()
    {
        moveComponent.EndJump();
    }

    public void Dodge()
    {
        if (_currentCharmEffects.Contains("대시마스터"))
        {
            _pawnAnimator.SetTrigger("Anim_Dodge");
            _invincibleTime = .25f;
            moveComponent.Dash();
        }
    }

    public void Move(Vector2 input)
    {
        if (IsStuned())
        {
            moveComponent.MovementUpdate(Vector2.zero);
        }
        else
        {
            moveComponent.MovementUpdate(input);
        }
    }

    public void AddItem(CharmInstance item)
    {
        for(int i = 0; i < _charmInventory.Length; i++)
        {
            if(_charmInventory[i] == null)
            {
                _charmInventory[i] = item;
                break;
            }
        }
    }

    public void TryInteract()
    {
        if (isDead) return;
        //콜리전 체크에서 검사하고 싶어서...
        //_tryInteract = true;
        Collider2D[] allOverlap = Physics2D.OverlapCircleAll(transform.position, 5f);

        foreach (Collider2D collider in allOverlap)
        {

            //사용가능하면 사용
            IInteractable interactable;

            if (collider.gameObject.TryGetComponent<IInteractable>(out interactable))
            {
                interactable.Interact(this);
                break;
            }
        }

    }

    //장착버튼누를떄발생
    /// <summary>
    /// 장착한 아이템이 있으면 해제후 아이템 반환
    /// 없으면 null반환만함
    /// </summary>
    /// <param name="equipIndex"></param>
    /// <returns></returns>
    public CharmInstance EquipUnequipCharm(int equipIndex)
    {
        //만약 장착템이 있으면 장착 해제 후 해제한 아이템 반환
        CharmInstance selectedCharm = _equippedCharms[equipIndex];
        if (selectedCharm != null)
        {
            _equippedCharms[equipIndex] = null;
            //그리고 아이템 인벤에 추가
            //CharmInstance foundItem = Array.Find(_charmInventory, x => x == selectedCharm );
            AddItem(selectedCharm);
            return selectedCharm;
        }
        else
        {
            return null;
        }
    }

    public CharmInstance TryEquipCharm(int charmIndex)
    {
        //장착시도 후 가능이면 까매지고 장착됨
        CharmInstance selectedCharm = _charmInventory[charmIndex];
        if (selectedCharm != null)
        {
            for (int i = 0; i < _equippedCharms.Length; i++)
            {
                if (_equippedCharms[i] == null)
                {
                    _equippedCharms[i] = selectedCharm;
                    _charmInventory[charmIndex] = null;
                    break;
                }
            }
            RecalcCharmEffect();
            return selectedCharm;
        }
        return null;
    }

    public void UpdateRespawnPoint(RespawnPoint respawn)
    {
        _respawnPoint = respawn;
    }

    public CharmInstance CharmAt(int charmIndex)
    {
        return _charmInventory[charmIndex];
    }

    public CharmInstance EquppedCharmAt(int equipIndex)
    {
        return _equippedCharms[equipIndex];
    }

    public bool IsItemEquipped(CharmInstance currentCharm)
    {
        CharmInstance foundItem = Array.Find(_equippedCharms, x => x == currentCharm);
        return (foundItem != null) ? true : false;

    }


    //가진 아이템들의 능력을 확성화
    //이름을 가지고 있으면 효과 ON
    //깨진 아이템은 추가 안함

    public void RecalcCharmEffect()
    {
        _currentCharmEffects.Clear();

        foreach (CharmInstance equippedCharm in _equippedCharms)
        {
            if (  equippedCharm == null|| equippedCharm.Braked == true)
            {
                continue;
            }
            _currentCharmEffects.Add(equippedCharm.CharmType.ItemName);
        }

        maxHp = initialMaxHp + (_currentCharmEffects.Contains("허술한 심장") ? 2 : 0);
        item_Damage = _currentCharmEffects.Contains("허술한 힘") ? 3f : 0f;
        item_attackSpeed = _currentCharmEffects.Contains("빠른 참격") ? .5f : 0f;
        item_hitInvincible = _currentCharmEffects.Contains("튼튼한 껍데기") ? .3f : 0f;
    }

    private void AttackKnockback(Collider2D attackCol, Collider2D otherCol)
    {
        if(_knockBackTime > 0f)
            return;
        List<Collider2D> result = new List<Collider2D>();
        result.Clear();
        attackCol.OverlapCollider(new ContactFilter2D().NoFilter(), result);
        //Todo 제대로 만들기
        string[] tags = new string[result.Count];
        for (int i =0; i < result.Count; i++)
        {
            tags[i] = result[i].tag;
        }
        //enemy,ground,spike
        if (tags.Contains("Spike"))
        {
            _attackDir = _controller.GetAttackDir();
            moveComponent.KnockBack(-_attackDir, 20f);
            _knockBackTime = .1f;
        }
        else if (tags.Contains("Enemy"))
        {
            _attackDir = _controller.GetAttackDir();
            moveComponent.KnockBack(-_attackDir, 20f);
            _knockBackTime = .1f;
        }
        else if (tags.Contains("Obstacle"))
        {
            _attackDir = _controller.GetAttackDir();
            moveComponent.KnockBack(-_attackDir, 15f);
            _knockBackTime = .1f;
        }
        else if (tags.Contains("Ground"))
        {
            _attackDir = _controller.GetAttackDir();
            moveComponent.KnockBack(-_attackDir, 10f);
            _knockBackTime = .1f;
        }
    }

    private bool IsInvincible()
    {
        return _invincibleTime > 0f;
    }
    private bool IsStuned()
    {
        return _stunTime > 0f;
    }

    public float GetHp()
    {
        return hp;
    }

    public void TakeDamage(float damage, Vector2 Attackerpos)
    {
        if(isDead) return;
        //invincible(damaged or dashing)
        if (_invincibleTime > 0f) { return; }
        hp-=Mathf.Ceil((damage / damagePerSlot));
        hud.RefreshAll();
        if(hp <= 0f)
        {
            isDead = true;
        }
        if (isDead)
        {
            _pawnAnimator.SetTrigger("Anim_Dead");
            _pawnAnimator.SetBool("Anim_IsDead", true);
            moveComponent.Dead();

            DeadEffect.SetActive(false);
            HitEffect.SetActive(false);
            DeadEffect.SetActive(true);
            HitEffect.SetActive(true);
        }
        else
        {
            _pawnAnimator.SetTrigger("Anim_Damaged");

            _stunTime = .6f;
            _invincibleTime = 1f + item_hitInvincible;

            HitEffect.SetActive(false);
            HitEffect.SetActive(true);
        }

    }

    //IFightable
    public void DealFixedDamage(IFightable target, float damage)
    {
        //Todo AttackCollider 끼리 닿으면 null
        if(target == null)
        {
            Debug.Log("Null");
            return;
        }
        target.TakeDamage((int)(damage + item_Damage) , transform.position);
        
    }

    public void DealDamage(IFightable target, float damage)
    {
        target.TakeDamage(damage , transform.position);
    }

    public void AddCoin(int count)
    {
        coinCount += count;
        hud.RefreshAll();
    }

    public int GetMoney()
    {
        return coinCount;
    }

    public void AddMp(float value)
    {
        mp += value;
        hud.RefreshAll();
    }
    public float GetMp()
    {
        return mp;
    }

    public void OnAttackSuccess(Collider2D attackCol, Collider2D collision)
    {
        AttackKnockback(attackCol ,collision);
        if (collision.CompareTag("Enemy"))
        {
            AddMp(15.0f);
        }
    }

    public void OpenShopHud(Shop shop)
    {
        shopUi.Init(shop);
        shopUi.SetActive(true);
    }

    public void SetSpawnPoint(RespawnPoint respawnPoint)
    {
        _respawnPoint = respawnPoint;
    }

    public void OnDeadAnimEvent()
    {
        Invoke("Respawn", 5f);
        UiManager.Instance.DeadUiOn();
    }

    public void Respawn()
    {
        UiManager.Instance.DeadUiOff();
        _rigidbody.velocity = Vector3.zero;
        //reset
        coinCount = 0;
        hp = maxHp;
        mp = 0f;
        isDead = false;
        _pawnAnimator.SetTrigger("Anim_Reset");
        if(_respawnPoint != null)
        {
            transform.position = _respawnPoint.position;
        }
        hud.RefreshAll();
    }

    public void SetSpikeRespawnPoint(SpikeRespawn spikeRespawn)
    {
        _spikeRespawn = spikeRespawn;
    }
    public void RespawnWhenSpike()
    {
        if(isDead) { return; }
        _rigidbody.velocity = Vector3.zero;
        _pawnAnimator.SetTrigger("Anim_Reset");
        transform.position = _spikeRespawn.position;
    }
}
