
using UnityEngine;
using System.Collections;

/// <summary>
/// NormalSkill.cs
/// クラス説明
/// ノーマルキャラのスキル
///
/// 作成日: 9/25
/// 作成者: 山田智哉
/// </summary>
public class NormalSkill : MonoBehaviour, ISkill
{


    public void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        Debug.Log("ノーマルのスキル");
    }

}