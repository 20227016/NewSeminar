using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingAttack : BaseEnemy
{
    [Tooltip("���̃_���[�W")]
    [SerializeField] private float _damage = 10f;

    /// <summary>
    /// HP��0�ȉ��ɂȂ�����Ă΂�鏈��(Base�Q��)
    /// </summary>
    protected override void OnDeath()
    {

    }
}