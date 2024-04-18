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

    //������
    public bool isJumping = false;
    public bool isDead = false;
    //���� ����
    /// <summary>�̰� ���� �ִϸ��̼� �̺�Ʈ���� ó���� </summary>
    public bool isPendingAttack = false;
    public bool isAttacking = false;
    private Vector2 _attackDir;
    private float _attackingTime = 0f;
    private int continuableAttackCount = 0;

    //ȸ��,����
    private float _invincibleTime = 0f;
    private float _stunTime = 0f;
    public float DodgeInvincibleTime = 0.5f;

    //ü��
    public float hp = 4f;
    public int initialMaxHp = 4;
    public float maxHp = 4f;
    public float mp = 0f;
    public float maxMp = 100f;
    //�������
    Vector2 curLocScale;

    //������
    private float item_Damage = 0f;
    private float itemAttackSpeedBounus = 0f;
    private float item_hitInvincible = 0f;

    //�κ��丮 //Todo: �κ� ����׿� public, �� �����ϱ�
    private CharmInstance[] _equippedCharms = new CharmInstance[5];
    public CharmData[] debugCharms;
    // Wow
    //public���� �����ϸ� null�� �ʱ�ȭ�����ʰ� �⺻ �����ڰ� ȣ��Ǵµ�??
    //�ٵ� �⺻�����ڰ� �����ٵ�
    //�⺻������ �׽�Ʈ
    //ItemInstance test = new ItemInstance();//�⺻������ ���� �ȵǴµ� �μ��� null�� �ֳ�?
    //ItemInstance test2 = new ItemInstance(null);// �Ƹ� �̷��� ���� �ʾ�����
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

    // Start is called before the first frame update
    void Awake()
    {
        //Todo �ڽĿ� ��Ʈ�ڽ��� ���� ����� �ʹ� 
        //���� ��ü���� ã�� �Ⱦ ���ϵ忡�� ������Ʈ�� �ٷ� �����ͺ�

        //�� �ڽ��� �ݸ����� ���� �˻����
        //�ڽ��� �ݸ����� �̸��������� �н�
        //Neverminer  transform.Find�ϸ� �̸����� ã�� �� �ִ�
        /*
         * ��뿹
         * ã�� ���� �ϱ� ���� ����� ã�ƺ���
         * Enum����?
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
        //�Է� ���⿡���� ��������Ʈ ���� ����
        if(isDead) return;


        //���� ���� �ð��� �ڵ忡�� ����
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



        if (_attackingTime + _knockBackTime + _stunTime <= 0)
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
        if (isDead) return;
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
        //�������� ��⸦ �ִϸ��̼� �̺�Ʈ�� �����̳����� üũ�ϰ� attaking �� ��������
        //������ �ִϸ��̼� ������ �ʱ�ȭ�� AllSlash �ִϸ��̼� EventOnAttackInterrupted ��ũ��Ʈ�� OnStateExit ����

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
        if (_currentCharmEffects.Contains("��ø�����"))
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
            moveComponent.OnDamaged();
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
        //�ݸ��� üũ���� �˻��ϰ� �;...
        //_tryInteract = true;
        Collider2D[] allOverlap = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (Collider2D collider in allOverlap)
        {

            //��밡���ϸ� ���
            IInteractable interactable;

            if (collider.gameObject.TryGetComponent<IInteractable>(out interactable))
            {
                interactable.Interact(this);
                break;
            }
        }

    }

    //������ư�������߻�
    /// <summary>
    /// ������ �������� ������ ������ ������ ��ȯ
    /// ������ null��ȯ����
    /// </summary>
    /// <param name="equipIndex"></param>
    /// <returns></returns>
    public CharmInstance EquipUnequipCharm(int equipIndex)
    {
        //���� �������� ������ ���� ���� �� ������ ������ ��ȯ
        CharmInstance selectedCharm = _equippedCharms[equipIndex];
        if (selectedCharm != null)
        {
            _equippedCharms[equipIndex] = null;
            //�׸��� ������ �κ��� �߰�
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
        //�����õ� �� �����̸� ������� ������
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


    //���� �����۵��� �ɷ��� Ȯ��ȭ
    //�̸��� ������ ������ ȿ�� ON
    //���� �������� �߰� ����

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

        maxHp = initialMaxHp + (_currentCharmEffects.Contains("����� ����") ? 2 : 0);
        item_Damage = _currentCharmEffects.Contains("����� ��") ? 3f : 0f;
        itemAttackSpeedBounus = _currentCharmEffects.Contains("���� ����") ? .5f : 0f;
        item_hitInvincible = _currentCharmEffects.Contains("ưư�� ������") ? .3f : 0f;
    }

    public void AttackKnockback(Collider2D attackCol, List<Collider2D> otherCol)
    {
        if(_knockBackTime > 0f)
            return;
        //���� ��������
        string[] tags = new string[otherCol.Count];
        for (int i =0; i < otherCol.Count; i++)
        {
            tags[i] = otherCol[i].tag;
        }
        //���Ѱ� �켱 ó��
        //enemy,ground,spike


        //����ִ��� Ȯ���ؾ��� �̰� ��ٷӴ� �±׷� �ϴ� ���� �׷� contain ���� ����
        //�ϳ��� ���ư��� �ִ밪�� ��������
        float maxKnockBack = 0f;

        foreach (Collider2D item in otherCol)
        {
            if (item.CompareTag("Spike"))
            {
                //��� Ư���� ã�� ���� ������
                maxKnockBack = 1f;
                break;
            }
            else if (item.CompareTag("Enemy"))
            {
                IFightable fightable;
                item.TryGetComponent(out fightable);

                if (fightable?.GetCombatComponent().IsDead() ?? true)
                    continue;
                maxKnockBack = .8f;
            }
            else if (item.CompareTag("Obstacle"))
            {
                maxKnockBack = .5f;
            }
            else if (item.CompareTag("Ground"))
            {
                maxKnockBack = .3f;
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
        hp = maxHp;
        mp = 0f;
        isDead = false;
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
        if(isDead) { return; }
        _rigidbody.velocity = Vector3.zero;
        _pawnAnimator.SetTrigger("Anim_Reset");
        transform.position = _spikeRespawn.position;
    }

    public void OnAttackSuccess(CombatComponent target)
    {
        if(target._owner.CompareTag("enemy"))
            mp += 15f;
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
