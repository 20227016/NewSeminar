using System;
using UniRx;
using UnityEngine;

/// <summary>
/// FighterCharacter.cs
/// クラス説明
/// ファイターのベース
///
/// 作成日: 9/30
/// 作成者: 山田智哉
/// </summary>
public class FighterCharacter : CharacterBase
{

    public override void AttackHit()
    {
        base.AttackHit();

        _passive.Passive(this);
    }
}