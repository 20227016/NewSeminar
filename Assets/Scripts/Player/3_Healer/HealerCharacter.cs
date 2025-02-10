using System;
using UniRx;
using UnityEngine;
using Fusion;

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

    private AudioSource _audioSource = default;

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

        float animationDuration = _animation.GetAnimationLength(_animator, _characterAnimationStruct._skillAnimation.name);
        RPC_TriggerAnimation(_characterAnimationStruct._skillAnimation.name);

        NetworkObject effect = Runner.Spawn(Effects[4], transform.position, Quaternion.identity);
        _audioSource = effect.GetComponent<AudioSource>();
        if (_audioSource != null)
        {
            // 効果音
            _sound.ProduceSE(_audioSource, _characterSoundStruct._attack_Special, _characterSoundStruct._playBackSpeed_Special, _characterSoundStruct._audioVolume_Special, _characterSoundStruct._delay_Special);


        }
        else
        {
            Debug.LogError($"オーディオソースが見つかりません");
        }

        Invincible(animationDuration);
        ResetState(animationDuration);
    }

}