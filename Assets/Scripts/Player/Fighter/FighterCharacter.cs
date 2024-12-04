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

    public override void AttackHit()
    {
        base.AttackHit();

        _passive.Passive(this);
    }
}