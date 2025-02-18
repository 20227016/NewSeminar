using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingAttack : BaseEnemy
{
    [Tooltip("翼のダメージ")]
    [SerializeField] private float _damage = 10f;

    /// <summary>
    /// HPが0以下になったら呼ばれる処理(Base参照)
    /// </summary>
    protected override void OnDeath()
    {

    }
}