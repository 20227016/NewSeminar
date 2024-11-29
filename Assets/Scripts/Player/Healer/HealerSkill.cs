
using UnityEngine;
using System.Collections;

/// <summary>
/// HealerSkill.cs
/// クラス説明
///
///
/// 作成日: 9/30
/// 作成者: 山田智哉
/// </summary>
public class HealerSkill : MonoBehaviour, ISkill
{
    public void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        print("ヒーラーのスキル");
    }
}