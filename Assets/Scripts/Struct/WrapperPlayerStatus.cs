
public class WrapperPlayerStatus
{
    /// <summary>
    /// 最大体力
    /// </summary>
    private float _maxHP = 100f;
    /// <summary>
    /// 最大スタミナ
    /// </summary>
    private float _maxStamina = 100f;

    public float MaxHp { get => _maxHP; set => _maxHP = value; }
    public float MaxStamina { get => _maxStamina; set => _maxStamina = value; }
}