using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class Participant : NetworkBehaviour, IRoomController
{

    /// <summary>
    /// �������
    /// </summary>
    private RoomInfo _roomInfo = default;

    private NetworkObject _networkObject = default;

    private void Start()
    {

        _networkObject = this.GetComponent<NetworkObject>();
        if(_networkObject == null)
        {

            Debug.LogError($"�����ɂ��Ă���l�b�g���[�N�I�u�W�F�N�g��������܂���");

        }
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
       
    }

    /// <summary>
    /// �Q���҂��������Ƃ��Ƀ��[���Ǘ��ɒǉ�
    /// �Ăяo��:�z�X�g�ƃN���C�A���g
    /// ���s����:�z�X�g
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void RPC_ParticipantCountAdd()
    {

        await GetRoomAwait();
        Debug.Log($"�Ăяo�����I�u�W�F�N�g:{this.gameObject}");
        // �X�V
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount + 1;
        Debug.Log($"���[���Q���l���ύX:{_roomInfo.CurrentParticipantCount}");

    }

    /// <summary>
    /// �Q���҂����������Ƀ��[���Ǘ�����폜
    /// �Ăяo��:�z�X�g�ƃN���C�A���g
    /// ���s����:�z�X�g
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void RPC_ParticipantCountRemove()
    {

        await GetRoomAwait();
        Debug.Log($"�Ăяo�����I�u�W�F�N�g:{this.gameObject}");
        // �X�V
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount - 1;
        Debug.Log($"���[���Q���l���ύX:{_roomInfo.CurrentParticipantCount}");

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async Task GetRoomAwait()
    {

        while(_roomInfo == null)
        {

            await Task.Delay(100); // 100ms���ƂɃ`�F�b�N

        }
        

    }

}
