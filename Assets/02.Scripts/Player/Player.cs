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
    private float item_attackSpeed = 0f;
    private float item_hitInvincible = 0f;

    //�κ��丮 //Todo: �κ� ����׿� public, �� �����ϱ�
    private CharmInstance[] _equippedCharms = new CharmInstance[5];
    public CharmData debugCharm;
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

        _equippedCharms[0] = new CharmInstance(debugCharm);
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
            //�Է¾����� ���°����� ȸ��
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
        Collider2D[] allOverlap = Physics2D.OverlapCircleAll(transform.position, 5f);

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
            return selectedCharm;
        }
        else
        {
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
        item_attackSpeed = _currentCharmEffects.Contains("���� ����") ? .5f : 0f;
        item_hitInvincible = _currentCharmEffects.Contains("ưư�� ������") ? .3f : 0f;
    }

    private void AttackKnockback(Collider2D attackCol, Collider2D otherCol)
    {
        if(_knockBackTime > 0f)
            return;
        List<Collider2D> result = new List<Collider2D>();
        result.Clear();
        attackCol.OverlapCollider(new ContactFilter2D().NoFilter(), result);
        //Todo ����� �����
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
        //Todo AttackCollider ���� ������ null
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
