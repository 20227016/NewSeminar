using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class Participant : NetworkBehaviour, IRoomController
{

   

    /// <summary>
    /// �������
    /// </summary>
    private RoomInfo _roomInfo = default;

    /// <summary>
    /// �l�b�g���[�N�����i�[
    /// </summary>
    private NetworkRunner _networkRunner = default;

    /// <summary>
    /// �����̃l�b�g���[�N�I�u�W�F�N�g�R���|�[�l���g
    /// </summary>
    private NetworkObject _networkObject = default;

    public void Start()
    {

        Debug.Log($"Start�����Q�J�n: {this.GetType().Name}�N���X");
        InitialGet();
        // �������z�X�g�I�u�W�F�N�g�̎�
        if (_networkObject.HasStateAuthority)
        {

            Debug.Log($"�z�X�g����");
            _roomInfo = GameObject.Find("Room").GetComponent<RoomInfo>();
            if (_roomInfo == null)
            {

                Debug.LogError($"���[���Ǘ��N���X��������܂���");

            }
            // �f�[�^������
            _roomInfo.RoomName = _roomInfo.RoomName;
            _roomInfo.MaxParticipantCount = _roomInfo.MaxParticipantCount;
            _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount;

        }
        Debug.Log($"Start�����Q�I��: {this.GetType().Name}�N���X");

    }

    /// <summary>
    /// �����Ɏ擾���鏈��
    /// </summary>
    private void InitialGet()
    {

        _networkObject = this.GetComponent<NetworkObject>();
        if (_networkObject == null)
        {

            Debug.LogError($"�����ɂ��Ă���l�b�g���[�N�I�u�W�F�N�g��������܂���");

        }
        _networkRunner = GetRunner.GetRunnerMethod();
        if (_networkRunner == null)
        {

            Debug.LogError($"�����ɂ��Ă���l�b�g���[�N�����i�[��������܂���");

        }

    }

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
            Debug.LogError($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        else
        {

            name = $"{_roomInfo.CurrentParticipantCount}_Client";
            Debug.LogError($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        
        NetworkObject participant = _networkRunner.GetPlayerObject(playerRef);
         INameMemory iNameSet = participant.GetComponent< INameMemory>();
        if (iNameSet == null)
        {

            Debug.LogError($"���O��ݒ肷�邽�߂̃C���^�[�t�F�[�X��������܂���");

        }
        iNameSet.Name = name;
        iNameSet.RPC_NameUpdate();
        int index = _roomInfo.SetName(name);
        // �e�L�X�g�ݒ�
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Name = name;
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
        (string name, bool isRegistration)[] nameInfos = _roomInfo.RemoveName(_networkRunner.GetPlayerObject(playerRef).name);
        for (int i = 0; i < nameInfos.Length; i++)
        {

            // �q�G�����L�[��ɂ���I�u�W�F�N�g�w��
            int index = i + 1;
            // �e�L�X�g�ݒ�
            GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Name = $"{nameInfos[i].name}";
            // �e�L�X�g�X�V
            GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();

        }
        // �X�V
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount - 1;
        Debug.Log($"���݂̃��[���Q���l���ύX:{_roomInfo.CurrentParticipantCount}");

    }

    /// <summary>
    /// ���O�̕ύX
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void RPC_ParticipantReName(string newName, NetworkObject participantsObj)
    {

        if (!_networkRunner.IsServer)
        {

            return;

        }
        await GetRoomAwait();
        int index = _roomInfo.ReName(newName, participantsObj.name);
        Debug.Log($"{index}");

        // �e�L�X�g�ݒ�
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Name = $"{newName}";
        // �e�L�X�g�X�V
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();
    }

    /// <summary>
    /// _roomInfo�擾�܂őҋ@
    /// </summary>
    /// <returns></returns>
    private async Task GetRoomAwait()
    {

        while (_roomInfo == null)
        {

            await Task.Delay(100); // 100ms���ƂɃ`�F�b�N

        }


    }

}
