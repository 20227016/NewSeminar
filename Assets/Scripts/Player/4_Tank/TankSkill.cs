using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TankSkill : MonoBehaviour, ISkill
{
    [SerializeField, Tooltip("スキルによる防御力上昇倍率")]
    private float _defenceMaltiplier = 2.0f;

    public void Skill(CharacterBase characterBase, float skillTime)
    {
        // 元の攻撃速度を保持
        float originalDefence = characterBase._characterStatusStruct._defensePower;

        // 攻撃速度を一時的に変更
        characterBase._characterStatusStruct._defensePower *= _defenceMaltiplier;

        // skillTime後に元の攻撃速度に戻す
        Observable.Timer(TimeSpan.FromSeconds(skillTime))
            .Subscribe(_ =>
            {
                characterBase._characterStatusStruct._defensePower = originalDefence;
            });
    }
}
