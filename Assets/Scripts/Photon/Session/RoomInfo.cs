using Fusion;
using UnityEngine;
using System.Collections.Generic;
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
    public int CurrentParticipantCount { get; set; } = 0;
    // �ő�v���C���[��[�l�b�g���[�N��œ���]
    [Networked]
    public int MaxParticipantCount { get; set; } = 4;

    /// <summary>
    /// �Q���҂̏��
    /// </summary>
    private (string name, bool isRegistration , bool isReady)[] _nameInfos = new (string name, bool isRegistration, bool isReady)[] 
                                                                             { ("���O1",false,false),("���O2", false,false),("���O3", false,false),("���O4",false,false) };

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
            _nameInfos[i] = (name,true,_nameInfos[i].isReady);
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
    public string[] RemoveName(string name)
    {

        List<string> returnNames = new();
        for (int i = 0; i < _nameInfos.Length; i++)
        {

            // ���O����v���邩
            if (_nameInfos[i].name != name)
            {

                continue;

            }
            // �q�G�����L�[��ɂ���I�u�W�F�N�g�Ƃ��낦��
            int index = i + 1;
            _nameInfos[i] = ($"���O{index}", false, false);
            break;

        }
        Sort();
        foreach ((string name, bool isRegistration, bool isReady) nameInfo in _nameInfos)
        {

            returnNames.Add(nameInfo.name);

        }
        return returnNames.ToArray() ;

    }

    /// <summary>
    /// ���Ԃ𖄂߂�
    /// </summary>
    public void Sort()
    {

        Debug.Log("�\�[�g");
        for (int i = 1; i < _nameInfos.Length; i++)
        {

            /*   ���̃C���f�b�N�X�����g�����邩�O�̃C���f�b�N�X����̎��ɒʂ�*/
            if (!_nameInfos[i].isRegistration || _nameInfos[i - 1].isRegistration)
            {

                continue;

            }
            // ���𖄂߂�i�P�l���ʂ�̂ŕ����������J���Ă��邱�Ƃ͂Ȃ��j
            _nameInfos[i - 1] = _nameInfos[i];
            // �q�G�����L�[��ɂ���I�u�W�F�N�g�Ƃ��낦��
            int index = i + 1;
            _nameInfos[i] = ($"���O{index}", false, false);

        }

    }

    /// <summary>
    /// ���O�̕ύX
    /// </summary>
    /// <param name="newName"></param>
    /// <param name="targetName"></param>
    /// <returns></returns>
    public int ReName(string newName, string targetName)
    {

        for (int i = 0; i < _nameInfos.Length; i++)
        {

            // ���O����v���邩
            if (_nameInfos[i].name != targetName)
            {

                continue;

            }
            _nameInfos[i].name = newName;
            // �q�G�����L�[��ɂ���I�u�W�F�N�g�Ƃ��낦��
            int index = i + 1;
            return index;

        }
        return 0;

    }

    /// <summary>
    /// ������Ԃ�ς���
    /// </summary>
    public void ChangeReady(string targetName)
    {

        for (int i = 0; i < _nameInfos.Length; i++)
        {

            // ���O����v���邩
            if (_nameInfos[i].name != targetName)
            {

                continue;

            }
            _nameInfos[i].isReady = !_nameInfos[i].isReady;
            Debug.Log($"{_nameInfos[i].name}�̏�����{_nameInfos[i].isReady}��");

        }

    }

    public int ReadyGoCount()
    {

        int num = default;
        foreach ((string name, bool isRegistration, bool isReady) nameInfo in _nameInfos)
        {

            if (nameInfo.isReady)
            {

                num++;

            }

        }

        return num;

    }

    /// <summary>
    /// �o���ł��邩�̊m�F
    /// </summary>
    public bool GoCheck()
    {

        foreach((string name, bool isRegistration, bool isReady) nameInfo in _nameInfos)
        {

            if (nameInfo.isReady)
            {

                continue;

            }
            return false;

        }
        return true;

    }

}