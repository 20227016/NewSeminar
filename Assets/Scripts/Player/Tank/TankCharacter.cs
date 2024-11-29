
using UnityEngine;
using System.Collections;

/// <summary>
/// TankCharacter.cs
/// クラス説明
///
///
/// 作成日: 9/13
/// 作成者: 山田智哉
/// </summary>
public class TankCharacter : CharacterBase
{

    public override void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        _skill.Skill(characterBase, skillTime, skillCoolTime);
    }
}