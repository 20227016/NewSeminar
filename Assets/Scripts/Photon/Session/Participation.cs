using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class Participation : BaseRoom, IRoomController
{

    /// <summary>
    /// �Q���҂��������Ƃ��Ƀ��[���Ǘ��ɒǉ�
    /// �Ăяo��:�z�X�g�ƃN���C�A���g
    /// ���s����:�z�X�g
    /// </summary>
    public async void ParticipantAdd(PlayerRef playerRef)
    {

        // Room�N���X�擾�܂ő҂�
        await GetRoomAwait();
        Debug.Log($"�Q���҉��Z�����Q�J�n: {this.GetType().Name}�N���X");
        Debug.Log($"�Ăяo�����I�u�W�F�N�g:{this.gameObject}");
        // �l���X�V
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount + 1;
        Debug.Log($"���݂̃��[���Q���l���ύX:{_roomInfo.CurrentParticipantCount}");
        // �������̉��̖��O
        string name = "";
        if (_roomInfo.CurrentParticipantCount == 1)
        {

            name = $"{_roomInfo.CurrentParticipantCount}_Host";
            Debug.Log($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        else
        {

            name = $"{_roomInfo.CurrentParticipantCount}_Client";
            Debug.Log($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        
        NetworkObject participant = _networkRunner.GetPlayerObject(playerRef);
         INameMemory iNameMemory = participant.GetComponent< INameMemory>();
        if (iNameMemory == null)
        {

            Debug.LogError($"���O��ݒ肷�邽�߂̃C���^�[�t�F�[�X��������܂���");

        }
        iNameMemory.Name = name;
        iNameMemory.RPC_NameUpdate();
        int index = _roomInfo.SetName(name);
        // �e�L�X�g�ݒ�
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Character = name;
        // �e�L�X�g�X�V
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();
        Debug.Log($"�Q���҉��Z�����Q�I��: {this.GetType().Name}�N���X");

    }

    /// <summary>
    /// �Q���҂����������Ƀ��[���Ǘ�����폜
    /// �Ăяo��:�z�X�g�ƃN���C�A���g
    /// ���s����:�z�X�g
    /// </summary>
    public async void ParticipantRemove(PlayerRef playerRef)
    {

        await GetRoomAwait();
        // ���O�����������Ƃ̖��O�̔z������炤
        string[] nameInfos = _roomInfo.RemoveName(_networkRunner.GetPlayerObject(playerRef).name);
        for (int i = 0; i < nameInfos.Length; i++)
        {

            // �q�G�����L�[��ɂ���I�u�W�F�N�g�w��
            int index = i + 1;
            // �e�L�X�g�ݒ�
            GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Character = $"{nameInfos[i]}";
            // �e�L�X�g�X�V
            GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();

        }
        // �X�V
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount - 1;
        Debug.Log($"���݂̃��[���Q���l���ύX:{_roomInfo.CurrentParticipantCount}");

    }

}
