
/// <summary>
/// PlayerAttackProvider.cs
/// クラス説明
/// 攻撃インターフェースを渡すクラス
///
/// 作成日: 9/13
/// 作成者: 山田智哉
/// </summary>
public class PlayerAttackProvider : IAttackProvider
{
    public PlayerAttackLight _playerAttackLight = new PlayerAttackLight(ComboCounter.Instance);

    public PlayerAttackStrong _playerAttackStrong = new PlayerAttackStrong(ComboCounter.Instance);

    public IAttackLight GetAttackLight()
    {
        return _playerAttackLight;
    }

    public IAttackStrong GetAttackStrong()
    {
        return _playerAttackStrong;
    }
}