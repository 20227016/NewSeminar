
using UnityEngine;
using System.Collections;

/// <summary>
/// FighterCharacter.cs
/// クラス説明
///
///
/// 作成日: 9/30
/// 作成者: 山田智哉
/// </summary>
public class FighterCharacter : CharacterBase
{
    public override void AttackLight(Transform transform, float attackMultipiler)
    {
        base.AttackLight(transform, attackMultipiler);
        _passive.Passive(this);
    }

    public override void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        
    }


}