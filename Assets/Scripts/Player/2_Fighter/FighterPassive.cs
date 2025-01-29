using UnityEngine;

/// <summary>
/// FighterPassive.cs
/// クラス説明
///
///
/// 作成日: 9/30
/// 作成者: 山田智哉
/// </summary>
public class FighterPassive : MonoBehaviour, IPassive
{
    [SerializeField, Tooltip("パッシブによるHP回復量( 1回毎に )")]
    private int _passiveHealValue = 2;

    public void Passive(CharacterBase characterBase)
    {
        characterBase.RPC_ReceiveHeal(_passiveHealValue);
    }
}