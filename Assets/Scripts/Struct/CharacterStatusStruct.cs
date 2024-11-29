
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
    /// スキルポイント上限
    /// </summary>
    public float _skillPointUpperLimit;

    /// <summary>
    /// 蘇生時間
    /// </summary>
    public float _ressurectionTime;
}