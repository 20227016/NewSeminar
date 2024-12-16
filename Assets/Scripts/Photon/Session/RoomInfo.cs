using Fusion;
using UnityEngine;
/// <summary>
/// �l�b�g���[�N�Ŏg�p����ϐ��̊Ǘ�
/// ���݂̖���
/// �ERoomInfo�Ō��݂̕����̐l����CurrentPlayerCount�ɑ������(�����ϐ�)
/// </summary>
public class RoomInfo : NetworkBehaviour
{

    [Networked]
    public string RoomName { get; set; } = "Room1";
    // ���݂̃v���C���[��[�l�b�g���[�N��œ���]
    [Networked]
    public int CurrentParticipantCount { get; set; } = 0 ;
    // �ő�v���C���[��[�l�b�g���[�N��œ���]
    [Networked]
    public int MaxParticipantCount { get; set; } = 4;

    /// <summary>
    /// �Q���҂̖��O
    /// </summary>
    private (string name, bool isRegistration)[] _nameInfos = new (string name, bool isRegistration)[] { ("���O�P",false),("���O�Q", false),("���O�R", false),("���O�S",false) };

    /// <summary>
    /// ���O���Z�b�g���Z�b�g�����C���f�b�N�X��␳�����l��Ԃ�
    /// �␳�̓q�G�����L�[��ɂ���I�u�W�F�N�g�̖��O���w�肷�邽��
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int SetName(string name)
    {

        for (int i = 0; i < _nameInfos.Length; i++)
        {

            // ���O���o�^�ς݂�
            if (_nameInfos[i].isRegistration)
            {

                continue;

            }
            _nameInfos[i] = (name,true);
            // �q�G�����L�[��ɂ���I�u�W�F�N�g�Ƃ��낦��
            int index = i + 1;
            return index;

        }
        return 0;

    }

    /// <summary>
    /// ���O���폜
    /// </summary>
    /// <param name="name"></param>
    public (string name, bool isRegistration)[] RemoveName(string name)
    {

        for (int i = 0; i < _nameInfos.Length; i++)
        {

            // ���O����v���邩
            if (_nameInfos[i].name != name)
            {

                continue;

            }
            Debug.LogError("�폜�Ώ۔���");
            // �q�G�����L�[��ɂ���I�u�W�F�N�g�Ƃ��낦��
            int index = i + 1;
            _nameInfos[i] = ($"���O{index}", false);
            break;

        }
        Sort();
        return _nameInfos;

    }

    public int ReName(string newName, string targetName)
    {

        Debug.LogError("����");
        Debug.LogError($"���O�X�y�[�X��{_nameInfos.Length}");
        Debug.LogError($"���̖��O�F{targetName}�V���Ȗ��O�F{newName}");

        //for (int i = 1; i < 5; i++)
        //{

        //    // �e�L�X�g�ݒ�
        //    string memoryName = GameObject.Find($"Participant_{i}").GetComponent<TextMemory>().Name;


        //}
        for (int i = 0; i < _nameInfos.Length; i++)
        {

            // ���O����v���邩
            if (_nameInfos[i].name != targetName)
            {

                continue;

            }
            Debug.LogError("�����Ώ۔���");
            _nameInfos[i].name = newName;
            Debug.LogError($"���[�v�C���g��{i}");
            // �q�G�����L�[��ɂ���I�u�W�F�N�g�Ƃ��낦��
            int index = i + 1;
            return index;

        }
        Debug.LogError("�����Ώۖ�����");
        return 0;

    }

    /// <summary>
    /// ���Ԃ𖄂߂�
    /// </summary>
    public void Sort()
    {
        Debug.LogError("�\�[�g");
        for (int i = 1; i < _nameInfos.Length; i++)
        {

            /*   ���̃C���f�b�N�X�����g�����邩�O�̃C���f�b�N�X����̎��ɒʂ�*/
            if (!_nameInfos[i].isRegistration)
            {

                continue;

            }
            if (_nameInfos[i-1].isRegistration)
            {

                continue;

            }
            // ���𖄂߂�i�P�l���ʂ�̂ŕ����������J���Ă��邱�Ƃ͂Ȃ��j
            _nameInfos[i - 1] = _nameInfos[i];
            // �q�G�����L�[��ɂ���I�u�W�F�N�g�Ƃ��낦��
            int index = i + 1;
            _nameInfos[i] = ($"���O{index}", false);
            break;

        }

    }

}