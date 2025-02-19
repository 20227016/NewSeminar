using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TankSkill : MonoBehaviour, ISkill
{
    [SerializeField, Tooltip("�X�L���ɂ��h��͏㏸�{��(* �h���)")]
    private float _defenceMaltiplier = 2f;

    public void Skill(CharacterBase characterBase, float skillTime)
    {
        float originalDefence = characterBase._characterStatusStruct._defensePower;

        characterBase._characterStatusStruct._defensePower *= _defenceMaltiplier;

        // skillTime��Ɍ��̖h��͂ɖ߂�
        Observable.Timer(TimeSpan.FromSeconds(skillTime))
            .Subscribe(_ =>
            {
                characterBase._characterStatusStruct._defensePower = originalDefence;
            });
    }
}
