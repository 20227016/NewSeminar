using UnityEngine;
using UniRx;
using System;

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
    [SerializeField, Tooltip("スキルによる攻撃速度上昇倍率")]
    private float _attackSpeedMaltiplier = 2.0f;

    public void Skill(CharacterBase characterBase, float skillTime)
    {
        Debug.Log("ファイターのスキル");

        // 元の攻撃速度を保持
        float originalAttackSpeed = characterBase._characterStatusStruct._attackSpeed;

        // 攻撃速度を一時的に変更
        characterBase._characterStatusStruct._attackSpeed *= _attackSpeedMaltiplier;

        // skillTime後に元の攻撃速度に戻す
        Observable.Timer(TimeSpan.FromSeconds(skillTime))
            .Subscribe(_ =>
            {
                characterBase._characterStatusStruct._attackSpeed = originalAttackSpeed;
                Debug.Log("スキル効果終了");
            });
    }
}