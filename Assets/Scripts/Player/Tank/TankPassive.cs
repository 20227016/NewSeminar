using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TankPassive : MonoBehaviour, IPassive
{
    [SerializeField, Tooltip("�p�b�V�u�ɂ��K�[�h���̖h��㏸��( * �h���)")]
    private float _guardMultiplier = 2.0f;

    // ���s�񐔂��Ǘ�
    private int _executionCount = 0;

    // ���̖h��͂�ێ�
    private float _originalDefensePower;

    public void Passive(CharacterBase characterBase)
    {
        Debug.Log("�^���N�̃p�b�V�u");

        // ������s���Ɍ��̖h��͂��L�^
        if (_executionCount == 0)
        {
            _originalDefensePower = characterBase._characterStatusStruct._defensePower;
        }

        _executionCount++;

        // ����: �h��͂��㏸
        if (_executionCount % 2 == 1)
        {
            characterBase._characterStatusStruct._defensePower *= _guardMultiplier;
            Debug.Log($"�h��͂��㏸: {characterBase._characterStatusStruct._defensePower}");
        }
        // �������: �h��͂����ɖ߂�
        else
        {
            characterBase._characterStatusStruct._defensePower = _originalDefensePower;
            Debug.Log($"�h��͂����ɖ߂�: {characterBase._characterStatusStruct._defensePower}");
        }

    }
}
