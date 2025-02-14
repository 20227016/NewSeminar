using UnityEngine;

[System.Serializable]
public struct EnemyStatusStruct
{

    [Header("最大HP")]
    public int _maxHp; // 最大HP
    [Header("現在HP")]
    public int _hp; // 現在HP
    [Header("攻撃力")]
    public int _attackPower; // 攻撃力
    [Header("攻撃速度")]
    public int _attackPowerSpeed; // 攻撃速度
    [Header("移動速度")]
    public int _moveSpeed; // 移動速度
    [Header("防御力")]
    public float _defensePercentage; // 防御力
    [Header("攻撃クールタイム")]
    public int _attackCoolTime; // 攻撃クールタイム
    [Header("攻撃遅延タイム")]
    public int _attackDelayTime; // 攻撃遅延タイム
    [Header("ダウンタイム")]
    public int _downedTime; // ダウンタイム

}