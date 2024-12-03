
using UnityEngine;

[System.Serializable]
public struct CharacterStatusStruct
{
    /// <summary>
    /// 最大HP、最大スタミナ
    /// </summary>
    public WrapperPlayerStatus _playerStatus;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float _walkSpeed;

    /// <summary>
    /// ダッシュ速度
    /// </summary>
    public float _runSpeed;

    /// <summary>
    /// 回避距離
    /// </summary>
    public float _avoidanceDistance;

    /// <summary>
    /// 回避持続時間
    /// </summary>
    public float _avoidanceDuration;

    /// <summary>
    /// 攻撃倍率
    /// </summary>
    public float _attackMultipiler;

    /// <summary>
    /// 攻撃速度
    /// </summary>
    public float _attackSpeed;

    /// <summary>
    /// 防御力
    /// </summary>
    public float _defensePower;

    /// <summary>
    /// スキルタイム
    /// </summary>
    public float _skillTime;

    /// <summary>
    /// スキルクールタイム
    /// </summary>
    public float _skillCoolTime;

    /// <summary>
    /// カウンタータイム
    /// </summary>
    public float _counterTime;

    /// <summary>
    /// スキルポイント上限値
    /// </summary>
    public float _skillPointUpperLimit;

    /// <summary>
    /// 蘇生所要時間
    /// </summary>
    public float _ressurectionTime;

    /// <summary>
    /// スタミナ自動回復量
    /// </summary>
    public float _recoveryStamina;

    /// <summary>
    /// ダッシュ時スタミナ消費量
    /// </summary>
    public float _runStamina;

    /// <summary>
    /// 回避時スタミナ消費
    /// </summary>
    public float _avoidanceStamina;

}