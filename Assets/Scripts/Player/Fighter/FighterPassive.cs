
using UnityEngine;
using System.Collections;

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
    public void Passive(CharacterBase characterBase)
    {
        characterBase.ReceiveDamage(-2);
    }
}