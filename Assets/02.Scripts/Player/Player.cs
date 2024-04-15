using System;
using System.Collections.Generic;
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

    public CombatComponent combatComponent;
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
    private float hp = 4f;
    private float mp = 0f;
    //�������
    Vector2 curLocScale;

    //�κ��丮 //Todo: �κ� ����׿� public, �� �����ϱ�

    private DynamicInventory inventory;
    private CharmInstance[] _equippedCharms = new CharmInstance[6];
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

        inventory = new DynamicInventory();

        _equippedCharms[0] = new CharmInstance(debugCharm);
    }

    private void Start()
    {
        hud.UpdateUi();
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
    }


    public Vector2 GetPlayerVelocity()
    {
        return _rigidbody.velocity;
    }

    public void SetDrag(float drag)
    {
        _rigidbody.drag = drag;
    }



    public void Attack(Vector2 attackDir)
    {
        if (IsStuned() | IsAttacking())
        {
            return;
        }
        _attackingTime = .4f;
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

    public void OnAnimationAttackEnd()
    {
        if (isPendingAttack)
        {
            isPendingAttack = false;
            Attack(_controller.GetAttackDir());
        }
        else
        {
            isAttacking = false;
        }
    }

    //Collisions

    //�̰� �������� ���� ���� �浹 �˻縦 �ص� �浹�˻縦 �ѹ��ϰ� ���� ����
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if(_tryInteract)
    //    {
    //        //��밡���ϸ� ���
    //        IInteractable interactable;

    //        if (collision.gameObject.TryGetComponent<IInteractable>(out interactable))
    //        {
    //            interactable.Interact(this);
    //            _tryInteract = false;
    //        }
    //    }
    //}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttackCollider"))
        {
            return;
        }
    }

    public void StartJump()
    {
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
        _pawnAnimator.SetTrigger("Anim_Dodge");
        _invincibleTime = .25f;
        moveComponent.Dash();
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

    internal void SetVisibleSelfOnMiniMap(bool v)
    {
        //playerMiniMap.ExposePlayer(true);
    }

    public void AddItem(ItemInstance item)
    {
        inventory.AddItem(item);
    }

    public void TryInteract()
    {
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
            for (int i = 0; i < _charmInventory.Length; i++)
            {
                //CharmInstance foundItem = Array.Find(_charmInventory, x => x == selectedCharm );
                if (_charmInventory[i] == null)
                {
                    _charmInventory[i] = selectedCharm;
                    break;
                }
            }
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
                    _charmInventory[i] = null;
                    break;
                }
            }
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


    //���� �����۵��� �ɷ��� Ȯ��ȭ
    //�̸��� ������ ������ ȿ�� ON
    //���� �������� �߰� ����

    public void RecalcCharmEffect()
    {
        _currentCharmEffects.Clear();

        foreach (CharmInstance equippedCharm in _equippedCharms)
        {
            if (equippedCharm.Braked == true)
            {
                continue;
            }
            _currentCharmEffects.Add(equippedCharm.CharmType.name);
        }
    }

    private void AttackKnockback(Collider2D other)
    {

        //enemy,ground,spike
        if (other.CompareTag("Spike"))
        {
            _attackDir = _controller.GetAttackDir();
            moveComponent.KnockBack(-_attackDir, 30f);
        }
        else if (other.CompareTag("Ground"))
        {
            _attackDir = _controller.GetAttackDir();
            moveComponent.KnockBack(-_attackDir, 10f);
        }
        else if (other.CompareTag("Enemy"))
        {
            _attackDir = _controller.GetAttackDir();
            moveComponent.KnockBack(-_attackDir, 15f);
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

    float IFightable.GetHp()
    {
        return combatComponent.GetHp();
    }

    void IFightable.TakeDamage(float damage, Vector2 Attackerpos)
    {

        //invincible(damaged or dashing)
        if (_invincibleTime > 0f) { return; }
        combatComponent.TakeDamage(Mathf.Ceil((damage / damagePerSlot)));
        hud.UpdateUi();
        if (combatComponent.IsDead())
        {
            _pawnAnimator.SetTrigger("Anim_Dead");
            _pawnAnimator.SetBool("Anim_IsDead", true);
            isDead = true;
        }
        else
        {
            _pawnAnimator.SetTrigger("Anim_Damaged");
            _stunTime = .6f;
            _invincibleTime = 1f;
        }

    }

    void IFightable.DealFixedDamage(IFightable target, float damage)
    {
        target.TakeDamage((int)(damage), transform.position);
    }

    void IFightable.DealDamage(IFightable target, float damage)
    {
        target.TakeDamage(damage, transform.position);
    }

    public void AddCoin(int count)
    {
        coinCount += count;
        hud.UpdateUi();
    }

    public int GetMoney()
    {
        return coinCount;
    }

    public void AddMp(float value)
    {
        mp += value;
        hud.UpdateUi();
    }
    public float GetMp()
    {
        return mp;
    }

    public void OnAttackSuccess(Collider2D collision)
    {
        AttackKnockback(collision);
        if (collision.CompareTag("Enemy"))
        {
            AddMp(15.0f);
        }
    }
}
