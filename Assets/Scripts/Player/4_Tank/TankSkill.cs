using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TankSkill : MonoBehaviour, ISkill
{
    [SerializeField, Tooltip("�X�L���ɂ��h��͏㏸�{��")]
    private float _defenceMaltiplier = 2.0f;

    public void Skill(CharacterBase characterBase, float skillTime)
    {
        // ���̍U�����x��ێ�
        float originalDefence = characterBase._characterStatusStruct._defensePower;

        // �U�����x���ꎞ�I�ɕύX
        characterBase._characterStatusStruct._defensePower *= _defenceMaltiplier;

        // skillTime��Ɍ��̍U�����x�ɖ߂�
        Observable.Timer(TimeSpan.FromSeconds(skillTime))
            .Subscribe(_ =>
            {
                characterBase._characterStatusStruct._defensePower = originalDefence;
            });
    }
}
