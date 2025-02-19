using UnityEngine;

public class TankPassive : MonoBehaviour, IPassive
{
    [SerializeField, Tooltip("�p�b�V�u�ɂ��K�[�h���̖h��͏㏸��(+ �h���)")]
    private float _guardDefencePower = 60f;

    // �K�[�h�t���O
    private bool _isGuard = false;

    public void Passive(CharacterBase characterBase)
    {
        TankCharacter tankCharacter = characterBase.GetComponent<TankCharacter>();

        // ��Ԃɉ����Ėh��͂�ύX
        if (!_isGuard)
        {
            // �K�[�h���h��͂�K�p
            tankCharacter.GuardDefencePower = _guardDefencePower;
        }
        else
        {
            // ���ɖ߂�
            tankCharacter.GuardDefencePower = 0;
        }

        // ��Ԃ�؂�ւ���
        _isGuard = !_isGuard;
    }
}
