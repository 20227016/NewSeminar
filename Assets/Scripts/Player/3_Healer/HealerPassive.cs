using UnityEngine;

/// <summary>
/// HealerPassive.cs
/// クラス説明
///
///
/// 作成日: 9/30
/// 作成者: 山田智哉
/// </summary>
public class HealerPassive : MonoBehaviour, IPassive
{
    [SerializeField, Tooltip("パッシブによる蘇生時間短縮( * 蘇生所要時間 )")]
    private float _ressurectionTimeShortening = 0.5f;

    // 元の蘇生所要時間
    private float _originalRessurectionTime = default;

    public void Passive(CharacterBase characterBase)
    {
        _originalRessurectionTime = characterBase._characterStatusStruct._ressurectionTime;

        characterBase._characterStatusStruct._ressurectionTime *= _ressurectionTimeShortening;
    }

}