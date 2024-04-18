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
        //충돌이 있다면
        if(_collider.enabled && _overlapResult.Count > 3)
        {
            //넉백 먼저
            _player.AttackKnockback(_collider, _overlapResult);
            // 일단 넉백 태그 순서로 넉백을 넣고
            // 데미지 필요하면 나중에 넣자
            foreach (Collider2D col in _overlapResult)
            {
                //충돌체중 태그가 맞는게 있다면 데미지 적용
                if (_damagableTag.Contains(col.tag))
                {
                    //데미지 인터페이스 
                    //근데 그냥 어택함수안에서 캐스팅하고 부르자 응집 시켜놓자
                    IFightable target;
                    col.gameObject.TryGetComponent(out target);
                    if (target == null)
                    {
                        //Assert.IsNotNull(target, $"it has damagable tag but not havin damageble component {col.gameObject.name}");groundprojectile
                        continue;
                    }
                    CombatComponent targetCombat = target.GetCombatComponent();
                    _owner.GetCombatComponent().DealDamage(targetCombat, _damage);
                }
            }
        }
    }
}
