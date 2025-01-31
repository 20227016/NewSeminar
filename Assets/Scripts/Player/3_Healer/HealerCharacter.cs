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
    protected override void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        // クールタイム中ならリターン
        if (_isSkillCoolTime) return;

        CurrentState = CharacterStateEnum.SKILL;

        // クールタイム管理
        _isSkillCoolTime = true;
        Observable.Timer(TimeSpan.FromSeconds(skillCoolTime))
            .Subscribe(_ => _isSkillCoolTime = false);

        // 発動後スキルポイントを０に
        _currentSkillPoint.Value = 0f;

        _skill.Skill(this, skillTime);

        float animationDuration = _animation.TriggerAnimation(_animator, _characterAnimationStruct._skillAnimation);

        Runner.Spawn(Effects[4], transform.position, Quaternion.identity);

        Invincible(animationDuration);
        ResetState(animationDuration);
    }
}