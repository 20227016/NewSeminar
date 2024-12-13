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

    public  void Start()
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
        if(_networkRunner == null)
        {

            Debug.LogError($"�����ɂ��Ă���l�b�g���[�N�����i�[��������܂���");

        }

    }

    /// <summary>
    /// �Q���҂��������Ƃ��Ƀ��[���Ǘ��ɒǉ�
    /// �Ăяo��:�z�X�g�ƃN���C�A���g
    /// ���s����:�z�X�g
    /// </summary>
    public async void ParticipantCountAdd(PlayerRef playerRef)
    {

        Debug.Log($"�Q���҉��Z�����Q�J�n: {this.GetType().Name}�N���X");
        // Room�N���X�擾�܂ő҂�
        await GetRoomAwait();
        Debug.Log($"�Ăяo�����I�u�W�F�N�g:{this.gameObject}");
        // �X�V
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount + 1;
        Debug.Log($"���݂̃��[���Q���l���ύX:{_roomInfo.CurrentParticipantCount}");
        string text = "";
        if (_roomInfo.CurrentParticipantCount == 1)
        {

            text = $"{_networkRunner.GetPlayerObject(playerRef).name}";
            Debug.LogError($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        else
        {

            text = $"{_networkRunner.GetPlayerObject(playerRef).name}";
            Debug.LogError($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        // �e�L�X�g�ݒ�
        GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<TextMemory>().Character = text;
        // �e�L�X�g�X�V
        GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<TextMemory>().RPC_TextUpdate();
        Debug.Log($"�Q���҉��Z�����Q�I��: {this.GetType().Name}�N���X");
    }

    /// <summary>
    /// �Q���҂����������Ƀ��[���Ǘ�����폜
    /// �Ăяo��:�z�X�g�ƃN���C�A���g
    /// ���s����:�z�X�g
    /// </summary>
    public async void ParticipantCountRemove(PlayerRef playerRef)
    {

        await GetRoomAwait();
        Debug.Log($"�Ăяo�����I�u�W�F�N�g:{this.gameObject}");
        // �e�L�X�g�ݒ�
        GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<TextMemory>().Character = $"���O";
        // �e�L�X�g�X�V
        GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<TextMemory>().RPC_TextUpdate();
        // �X�V
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount - 1;
        Debug.Log($"���݂̃��[���Q���l���ύX:{_roomInfo.CurrentParticipantCount}");

    }

    /// <summary>
    /// 
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
