using System;
using UniRx;
using UnityEngine;

/// <summary>
/// HealerCharacter.cs
/// クラス説明
/// ヒーラーのベース
/// 
/// 作成日: 9/30
/// 作成者: 山田智哉
/// </summary>
public class HealerCharacter : CharacterBase
{
    public override void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        if (_isSkillCoolTime) return;

        Observable.Timer(TimeSpan.FromSeconds(skillCoolTime))
            .Subscribe(_ =>
            {
                Debug.Log("スキルクールタイム終了");
                _isSkillCoolTime = false;
            });

        _isSkillCoolTime = true;

        _networkedSkillPoint = 0f;

        _skill.Skill(this, skillTime);
    }
}