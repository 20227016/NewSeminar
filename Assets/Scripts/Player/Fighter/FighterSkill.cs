
using UnityEngine;
using System.Collections;

/// <summary>
/// FighterSkill.cs
/// クラス説明
///
///
/// 作成日: 9/30
/// 作成者: 山田智哉
/// </summary>
public class FighterSkill : MonoBehaviour, ISkill
{
    public void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        print("ファイターのスキル");
    }
}