using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem.XR;

public class PlayerDamageTrigger : MonoBehaviour
{
    Player _player;
    private Collider2D _collider;

    [SerializeField] private List<string> _damagableTag = new List<string>();
    [SerializeField] private Character _owner;
    [SerializeField] private DamageType _damageType;
    [SerializeField] private float _damage;
    [SerializeField] private List<Collider2D> _overlapResult;

    internal void SetOwner(FalseKnight owner)
    {
        _owner = owner;
    }

    private void Awake()
    {
        _player = transform.GetComponentInParent<Player>();
        _collider = GetComponent<Collider2D>();
        Assert.IsNotNull(_collider);
        Assert.IsNotNull(_player);
        foreach (string tag in _damagableTag)
        {
            Assert.IsTrue(tag != "");
        }

    }

    private void Update()
    {
        _collider.OverlapCollider(new ContactFilter2D().NoFilter(), _overlapResult);
        //�浹�� �ִٸ�
        if(_overlapResult.Count > 3)
        {
            //�˹� ����
            _player.AttackKnockback(_collider, _overlapResult);
            // �ϴ� �˹� �±� ������ �˹��� �ְ�
            // ������ �ʿ��ϸ� ���߿� ����
            foreach (Collider2D col in _overlapResult)
            {
                //�浹ü�� �±װ� �´°� �ִٸ� ������ ����
                if (_damagableTag.Contains(col.tag))
                {
                    //������ �������̽� 
                    //�ٵ� �׳� �����Լ��ȿ��� ĳ�����ϰ� �θ��� ���� ���ѳ���
                    IFightable target;
                    col.gameObject.TryGetComponent(out target);
                    if (target == null)
                    {
                        //Assert.IsNotNull(target, $"it has damagable tag but not havin damageble component {col.gameObject.name}");groundprojectile
                        continue;
                    }
                    //������ �ְ� �� ���̸� ��
                    //������ ������ �±׸� �ִ��� �˻����⶧���� �׾����� ��Ҵ����� �𸥴�
                    //�������� �������ָ� ���
                    if(!target.IsDead())
                    {
                        _player.OnDamageSuccess();
                    }
                    switch (_damageType)
                    {
                        case DamageType.FixedDamage:
                            _owner.DealFixedDamage(target, _damage);
                            break;
                        //���� ��� �� ����
                        case DamageType.StatPostDamage:
                            _owner.DealDamage(target, _damage);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
