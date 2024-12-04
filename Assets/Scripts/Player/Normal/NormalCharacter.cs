using System;
using UniRx;
using UnityEngine;

/// <summary>
/// NormalCharacter.cs
/// クラス説明
/// ノーマルのベース
///
/// 作成日: 9/3
/// 作成者: 山田智哉
/// </summary>
public class NormalCharacter : CharacterBase
{

    public override void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        if (_isSkillCoolTime) return;

        Observable.Timer(TimeSpan.FromSeconds(skillCoolTime))
            .Subscribe(_ =>
            {
                Debug.Log("スキルクールタイム終了");
            });

        _isSkillCoolTime = true;

        _networkedSkillPoint = 0f;

        _skill.Skill(this, skillTime);
    }
}