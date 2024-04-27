using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
public class Player : Character, IFightable
{
    [field: Header("OnlyForDebugDon'tTouch")]
    [field: Header("ComponentsWithAutoInit")]
    [field: SerializeField] public PlayerController _controller { get; private set; }
    [field: SerializeField] public Rigidbody2D _rigidbody { get; private set; }
    [field: SerializeField] public Transform _pawnSprite { get; private set; }
    [field: SerializeField] public Animator _pawnAnimator { get; private set; }
    [field: SerializeField] public HudUi hud { get; private set; }
    [field: SerializeField] public PlayerMoveComponent moveComponent { get; private set; }
    [field: Space(30)]
    [field: Header("PlayersStates")]
    [field: SerializeField] public bool isJumping { get; set; } = false;
    //이 아래 전부 전투에 관련된 것들이다.
    //전투 클래스로 따로 뺀다면 장점이 있을까
    //현재 이벤트로 전투컴포넌트에 등록하여 사용하고 있다.
    [field: SerializeField] public bool isAttacking { get; set; } = false;
    [field: SerializeField] private Vector2 _attackDir { get; set; }
    [field: SerializeField] private float _attackingTime { get; set; } = 0f;
    [field: SerializeField] private int continuableAttackCount { get; set; } = 0;
    [field: SerializeField] private float _invincibleTime { get; set; } = 0f;
    [field: SerializeField] private float _stunTime { get; set; } = 0f;
    [field: SerializeField] public float DodgeInvincibleTime { get; set; } = 0.5f;
    [field: SerializeField] public float mp { get; set; } = 0f;
    [field: SerializeField] public float maxMp { get; set; } = 100f;
    [field: Header("ItemBonuses")]
    //아이템
    [field: SerializeField] private float item_Damage { get; set; } = 0f;
    [field: SerializeField] private float itemAttackSpeedBounus { get; set; } = 0f;
    [field: SerializeField] private float item_hitInvincible { get; set; } = 0f;
    [field: Header("CharmInventorys")]
    //인벤토리 
    [field: SerializeReference] private CharmInstance[] _equippedCharms { get; set; } = new CharmInstance[5];
    [field: SerializeField] public CharmData[] initialChrams;
    [field: SerializeReference] private CharmInstance[] _charmInventory { get; set; } = new CharmInstance[24];
    [field: SerializeField] private HashSet<string> _currentCharmBonuses { get; set; } = new HashSet<string>();
    [field: SerializeField] public int coinCount { get; set; } = 0;
    [field: SerializeField] public ShopUi shopUi { get; set; }
    [field: SerializeField] private float _knockBackTime { get; set; } = 0f;
    [field: Header("Respawns")]
    //Respawn
    [field: SerializeField] public RespawnPoint _respawnPoint { get; set; }
    [field: SerializeField] public GameObject HitEffect { get; set; }
    [field: SerializeField] public GameObject DeadEffect { get; set; }
    [field: SerializeField] public SpikeRespawn _spikeRespawn { get; set; }
    public System.Action OnPlayerRespawn { get; set; }
    [field: SerializeField] public float defaultAttackSpeed { get; set; } = .4f;
    [field: SerializeField] private CombatComponent _combatComponent { get; set; }
    [field: SerializeReference] private Animator[] _attackEffectAnimator { get; set; }
    [field: SerializeField] private float damagedKnockbackForce { get; set; } = 100f;
    void Awake()
    {
        _controller = GetComponent<PlayerController>();
        moveComponent = GetComponent<PlayerMoveComponent>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _pawnSprite = transform.GetComponentInChildren<Transform>();
        _pawnAnimator = transform.GetComponentInChildren<Animator>();
        Assert.IsNotNull(_controller);
        Assert.IsNotNull(moveComponent);
        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_pawnSprite);
        Assert.IsNotNull(_pawnAnimator);
        //event subscribe
        _combatComponent.OnDamaged += OnDamage;
        _combatComponent.OnDamagedWAttacker += OnAttackSuccess;
        _combatComponent.OnDead += OnDead;
        _combatComponent.OnHeal += OnHeal;
        _combatComponent.AdditionalDamageableCondition += addtionalCondition;
        _combatComponent.Init(transform);
        //initial Charm
        for (int i = 0; i < initialChrams.Length; i++)
        {
            _equippedCharms[i] = new CharmInstance(initialChrams[i]);
        }
        RecalcCharmEffect();
    }
    private void Start()
    {
        hud = UiManager.Instance.hudUi;
        Assert.IsNotNull(hud);
        hud.RefreshAll();
    }
    private void Update()
    {
        if (_combatComponent.IsDead()) return;
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
        else
        {
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
        //isAttackable
        if (_combatComponent.IsDead()) return;
        if (IsStuned() || IsAttacking())
        {
            return;
        }
        //set attacking
        _attackingTime = defaultAttackSpeed - itemAttackSpeedBounus;
        //play animation by input direction
        if (attackDir.y > .7f)
        {
            continuableAttackCount = 0;
            _pawnAnimator.SetTrigger("Anim_Attack_Slash_Up");
        }
        else if (attackDir.y < -.7f)
        {
            continuableAttackCount = 0;
            _pawnAnimator.SetTrigger("Anim_Attack_Slash_Down");
        }
        else
        {
            //call diffren animation time to time
            //reset after reset time
            if (continuableAttackCount % 2 == 0)
            {
                _pawnAnimator.SetTrigger("Anim_Attack_Slash");
            }
            else
            {
                _pawnAnimator.SetTrigger("Anim_Attack_Slash_Alt");
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
        _pawnAnimator.SetBool("Anim_IsJumping", true);
        _pawnAnimator.SetTrigger("Anim_Jump");
    }
    public void EndJump()
    {
        moveComponent.EndJump();
        _pawnAnimator.SetBool("Anim_IsJumping", false);
    }
    public void Dodge()
    {
        if (_currentCharmBonuses.Contains("대시마스터"))
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
    public void AddItem(CharmInstance item)
    {
        for (int i = 0; i < _charmInventory.Length; i++)
        {
            if (_charmInventory[i] == null)
            {
                _charmInventory[i] = item;
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
        _currentCharmBonuses.Clear();

        foreach (CharmInstance equippedCharm in _equippedCharms)
        {
            if (equippedCharm == null || equippedCharm.Braked == true)
            {
                continue;
            }
            _currentCharmBonuses.Add(equippedCharm.CharmType.ItemName);
        }

        _combatComponent.AddMaxHp(_currentCharmBonuses.Contains("허술한 심장") ? 2 : 0);
        item_Damage = _currentCharmBonuses.Contains("허술한 힘") ? 3f : 0f;
        itemAttackSpeedBounus = _currentCharmBonuses.Contains("빠른 참격") ? .15f : 0f;
        item_hitInvincible = _currentCharmBonuses.Contains("튼튼한 껍데기") ? .3f : 0f;
    }
    public void UpdateRespawnPoint(RespawnPoint respawn)
    {
        _respawnPoint = respawn;
    }

    public void AttackKnockback(Collider2D attackCol, List<Collider2D> otherCol)
    {
        if (_knockBackTime > 0f)
            return;
        //전부 가져오기
        string[] tags = new string[otherCol.Count];
        for (int i = 0; i < otherCol.Count; i++)
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
        if (_respawnPoint != null)
        {
            transform.position = _respawnPoint.position;
        }
        hud.RefreshAll();
        _controller.ResetControl();
        moveComponent.ResetMove();
        if (OnPlayerRespawn != null)
        {
            OnPlayerRespawn.Invoke();
        }
    }

    public void SetSpikeRespawnPoint(SpikeRespawn spikeRespawn)
    {
        _spikeRespawn = spikeRespawn;
    }
    public void RespawnWhenSpike()
    {
        if (_combatComponent.IsDead()) { return; }
        _rigidbody.velocity = Vector3.zero;
        _pawnAnimator.SetTrigger("Anim_Reset");
        transform.position = _spikeRespawn.position;
    }
    public void OnAttackSuccess(CombatComponent target)
    {
        //Todo 맞을떄차고있음 고치셈
        if (target._owner.CompareTag("Enemy"))
        {
            if (target.noManaRegenOnHit)
            {
                return;
            }
            mp += 15f;
            if (maxMp < mp)
            {
                mp = maxMp;
            }
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
        Vector3 knockbackDir = new Vector3((-_combatComponent.prevAttackersPos.x + transform.position.x), 0f, 0f).normalized;

        moveComponent.KnockBack(knockbackDir, damagedKnockbackForce);

        HitEffect.SetActive(false);
        HitEffect.SetActive(true);
    }
    private void OnHeal()
    {
        hud.RefreshAll();
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
        if (_invincibleTime > 0f)
        {
            return false;
        }
        return true;
    }
    public void Teleport(Vector3 position)
    {
        transform.position = position;
        GameManager.Instance.CameraToPlayer();
    }
}
