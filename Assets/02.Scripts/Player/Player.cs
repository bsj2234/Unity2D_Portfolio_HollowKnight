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
    //공격 관련
    /// <summary>이건 공격 애니메이션 이벤트에서 처리됨 </summary>
    public bool isPendingAttack = false;
    public bool isAttacking = false;
    private Vector2 _attackDir;
    [SerializeField]private float _attackingTime = 0f;
    private int continuableAttackCount = 0;

    //회피,무적
    private float _invincibleTime = 0f;
    private float _stunTime = 0f;
    public float DodgeInvincibleTime = 0.5f;

    public float mp = 0f;
    public float maxMp = 100f;
    //뒤집기용
    Vector2 curLocScale;

    //아이템
    private float item_Damage = 0f;
    private float itemAttackSpeedBounus = 0f;
    private float item_hitInvincible = 0f;

    //인벤토리 
    private CharmInstance[] _equippedCharms = new CharmInstance[5];
    public CharmData[] debugCharms;
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

    public System.Action OnPlayerReset;

    public float defaultAttackSpeed = .4f;

    public PlayerDamageTrigger _playerDamageTrigger;
    [SerializeField]private CombatComponent _combatComponent;
    [SerializeField]private Animator[] _attackEffectAnimator;

    [SerializeField] private float damagedKnockbackForce = 100f;

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

        _combatComponent.OnDamaged += OnDamage;
        _combatComponent.OnDamagedWAttacker += OnAttackSuccess;
        _combatComponent.OnDead += OnDead;
        _combatComponent.AdditionalDamageCondition += addtionalCondition;
        _combatComponent.Init(transform);

        //debugCharms
        for (int i  = 0; i < debugCharms.Length; i++)
        {
            _equippedCharms[i] = new CharmInstance(debugCharms[i]);
        }
        RecalcCharmEffect();
    }

    private void Start()
    {
        hud.RefreshAll();
    }

    private void Update()
    {
        //입력 방향에따라 스프라이트 방향 설정
        if(_combatComponent.IsDead()) return;


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
        if (_attackingTime > 0f) 
        { 
            _attackingTime -= Time.deltaTime;
            _pawnAnimator.speed = defaultAttackSpeed / (defaultAttackSpeed - itemAttackSpeedBounus);
            foreach (var item in _attackEffectAnimator)
            {
                item.speed = defaultAttackSpeed / (defaultAttackSpeed - itemAttackSpeedBounus);
            }
        }
        else { 
            continuableAttackCount = 0;
            _pawnAnimator.speed = 1f;
            foreach (var item in _attackEffectAnimator)
            {
                item.speed = 1f;
            }
        }
        if (_knockBackTime > 0f) { _knockBackTime -= Time.deltaTime; }



        if (_attackingTime <= 0f && _knockBackTime <= 0f && _stunTime <= 0)
        {
            moveComponent.isMovable = true;
            if (_controller.hasMoveInput)
            {
                _pawnAnimator.SetBool("Anim_IsRunning", true);
            }
            else
            {
                _pawnAnimator.SetBool("Anim_IsRunning", false);
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
        }
        else
        {
            moveComponent.isMovable = false;
        }


    }
    public void Attack(Vector2 attackDir)
    {
        if (_combatComponent.IsDead()) return;
        if (IsStuned() | IsAttacking())
        {
            return;
        }
        _attackingTime = defaultAttackSpeed - itemAttackSpeedBounus;
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
        if (_combatComponent.IsDead()) return;
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
        if (_combatComponent.IsDead()) return;
        //콜리전 체크에서 검사하고 싶어서...
        //_tryInteract = true;
        Collider2D[] allOverlap = Physics2D.OverlapCircleAll(transform.position, 1f);

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
            RecalcCharmEffect();
            return selectedCharm;
        }
        else
        {
            RecalcCharmEffect();
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

        _combatComponent.AddedMaxHp(_currentCharmEffects.Contains("허술한 심장") ? 2 : 0);
        item_Damage = _currentCharmEffects.Contains("허술한 힘") ? 3f : 0f;
        itemAttackSpeedBounus = _currentCharmEffects.Contains("빠른 참격") ? .15f : 0f;
        item_hitInvincible = _currentCharmEffects.Contains("튼튼한 껍데기") ? .3f : 0f;
    }

    public void AttackKnockback(Collider2D attackCol, List<Collider2D> otherCol)
    {
        if(_knockBackTime > 0f)
            return;
        //전부 가져오기
        string[] tags = new string[otherCol.Count];
        for (int i =0; i < otherCol.Count; i++)
        {
            tags[i] = otherCol[i].tag;
        }
        //강한것 우선 처리
        //enemy,ground,spike


        //살아있는지 확인해야함 이게 까다롭다 태그로 하다 보니 그럼 contain 쓰지 말고
        //하나씩 돌아가며 최대값을 저장하자
        float maxKnockBack = 0f;

        foreach (Collider2D item in otherCol)
        {
            if (item.CompareTag("Spike"))
            {
                //얘는 특별히 찾자 마자 나가자
                maxKnockBack = 1f;
                break;
            }
            else if (item.CompareTag("Enemy"))
            {
                IFightable fightable;
                item.TryGetComponent(out fightable);

                if (fightable?.GetCombatComponent().IsDead() ?? true)
                    continue;
                maxKnockBack = Mathf.Max(maxKnockBack, .8f);
            }
            else if (item.CompareTag("Obstacle"))
            {
                maxKnockBack = Mathf.Max(maxKnockBack, .7f);
            }
            else if (item.CompareTag("Ground"))
            {
                maxKnockBack = Mathf.Max(maxKnockBack, .6f);
            }
        }
        _attackDir = _controller.GetAttackDir();
        moveComponent.KnockBack(-_attackDir, maxKnockBack, defaultAttackSpeed - itemAttackSpeedBounus);
        _knockBackTime = .1f;
    }

    private bool IsInvincible()
    {
        return _invincibleTime > 0f;
    }
    private bool IsStuned()
    {
        return _stunTime > 0f;
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
        _combatComponent.ResetDead();
        mp = 0f;
        _pawnAnimator.SetTrigger("Anim_Reset");
        if(_respawnPoint != null)
        {
            transform.position = _respawnPoint.position;
        }
        hud.RefreshAll();
        _controller.ResetControl();
        moveComponent.ResetMove();
        if(OnPlayerReset != null)
        {
            OnPlayerReset.Invoke();
        }
    }

    public void SetSpikeRespawnPoint(SpikeRespawn spikeRespawn)
    {
        _spikeRespawn = spikeRespawn;
    }
    public void RespawnWhenSpike()
    {
        if(_combatComponent.IsDead()) { return; }
        _rigidbody.velocity = Vector3.zero;
        _pawnAnimator.SetTrigger("Anim_Reset");
        transform.position = _spikeRespawn.position;
    }

    public void OnAttackSuccess(CombatComponent target)
    {
        //Todo 맞을떄차고있음 고치셈
        if (target._owner.CompareTag("Enemy"))

        {
            mp += 15f;
            hud.RefreshAll();
        }

    }

    public override CombatComponent GetCombatComponent()
    {
        return _combatComponent;
    }

    public void OnDamage()
    {

        //invincible(damaged or dashing)
        hud.RefreshAll();
        _pawnAnimator.SetTrigger("Anim_Damaged");

        _stunTime = .15f;
        _invincibleTime = .9f + item_hitInvincible;

        //knockback
        Vector3 knockbackDir = (-_combatComponent.prevAttackersPos + transform.position).normalized;

        moveComponent.KnockBack(knockbackDir, damagedKnockbackForce);

        HitEffect.SetActive(false);
        HitEffect.SetActive(true);
    }

    public void OnDead()
    {

        _pawnAnimator.SetTrigger("Anim_Dead");
        _pawnAnimator.SetBool("Anim_IsDead", true);
        moveComponent.Dead();

        DeadEffect.SetActive(false);
        HitEffect.SetActive(false);
        DeadEffect.SetActive(true);
        HitEffect.SetActive(true);
    }

    public bool addtionalCondition()
    {
        if(_invincibleTime > 0f)
        {
            return false;
        }
        return true;
    }
}
