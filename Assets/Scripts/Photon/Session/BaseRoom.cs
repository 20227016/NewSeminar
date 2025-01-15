using Fusion;
using System.Threading.Tasks;
using UnityEngine;


public class BaseRoom : NetworkBehaviour
{

    /// <summary>
    /// �������
    /// </summary>
    protected RoomInfo _roomInfo = default;

    /// <summary>
    /// �l�b�g���[�N�����i�[
    /// </summary>
    protected NetworkRunner _networkRunner = default;

    /// <summary>
    /// �����̃l�b�g���[�N�I�u�W�F�N�g�R���|�[�l���g
    /// </summary>
    protected NetworkObject _networkObject = default;

    public override void Spawned()
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
        _roomInfo = GameObject.Find("Room").GetComponent<RoomInfo>();
        if (_roomInfo == null)
        {

            Debug.LogError($"{this.gameObject}�Ń��[���Ǘ��N���X��������܂���");

        }
        // �f�[�^������
        _roomInfo.RoomName = _roomInfo.RoomName;
        _roomInfo.MaxParticipantCount = _roomInfo.MaxParticipantCount;
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount;

    }

    /// <summary>
    /// _roomInfo�擾�܂őҋ@
    /// </summary>
    /// <returns></returns>
    protected async Task GetRoomAwait()
    {

        while (_roomInfo == null)
        {

            await Task.Delay(100); // 100ms���ƂɃ`�F�b�N

        }

    }

}
