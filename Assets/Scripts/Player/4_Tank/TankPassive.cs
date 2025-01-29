using UnityEngine;

public class TankPassive : MonoBehaviour, IPassive
{
    [SerializeField, Tooltip("�p�b�V�u�ɂ��K�[�h���̖h��͏㏸��( * �h���)")]
    private float _guardMultiplier = 2.0f;

    // �K�[�h�t���O
    private bool _isGuard = false;

    // ���̖h��͂�ێ�
    private float _originalDefensePower;

    public void Passive(CharacterBase characterBase)
    {
        // ������s���Ɍ��̖h��͂��L�^
        if (!_isGuard && _originalDefensePower == 0)
        {
            _originalDefensePower = characterBase._characterStatusStruct._defensePower;
        }

        // ��Ԃɉ����Ėh��͂�ύX
        if (!_isGuard)
        {
            // �h��͂��㏸
            characterBase._characterStatusStruct._defensePower *= _guardMultiplier;
        }
        else
        {
            // �h��͂����ɖ߂�
            characterBase._characterStatusStruct._defensePower = _originalDefensePower;
        }

        // ��Ԃ�؂�ւ���
        _isGuard = !_isGuard;
    }
}
