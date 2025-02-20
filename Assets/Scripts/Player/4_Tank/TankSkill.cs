using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TankSkill : MonoBehaviour, ISkill
{
    [SerializeField, Tooltip("スキルによる防御力上昇倍率(* 防御力)")]
    private float _defenceMaltiplier = 2f;

    public void Skill(CharacterBase characterBase, float skillTime)
    {
        float originalDefence = characterBase._characterStatusStruct._defensePower;

        characterBase._characterStatusStruct._defensePower *= _defenceMaltiplier;

        // skillTime後に元の防御力に戻す
        Observable.Timer(TimeSpan.FromSeconds(skillTime))
            .Subscribe(_ =>
            {
                characterBase._characterStatusStruct._defensePower = originalDefence;
            });
    }
}
