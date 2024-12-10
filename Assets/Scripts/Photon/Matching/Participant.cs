using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class Participant : NetworkBehaviour,IParticipantInfo
{

    /// <summary>
    /// �������
    /// </summary>
    private RoomInfo _roomInfo = default;

    /// <summary>
    /// �����̃l�b�g�I�u�W�F�N�g
    /// </summary>
    private NetworkObject _netObj = default;

    /// <summary>
    /// �Q���҂��������Ƃ��Ƀ��[���Ǘ��ɒǉ�
    /// �Ăяo��:�z�X�g�ƃN���C�A���g
    /// ���s����:�z�X�g
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void RPC_ParticipantCountAdd()
    {

        Debug.Log($"�Ăяo�����I�u�W�F�N�g:{this.gameObject}");
        // �f�[�^������
        _roomInfo.RoomName = _roomInfo.RoomName;
        _roomInfo.MaxParticipantCount = _roomInfo.MaxParticipantCount;
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount;
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

        Debug.Log($"�Ăяo�����I�u�W�F�N�g:{this.gameObject}");
        // �f�[�^������
        _roomInfo.RoomName = _roomInfo.RoomName;
        _roomInfo.MaxParticipantCount = _roomInfo.MaxParticipantCount;
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount;
        // �X�V
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount + 1;
        Debug.Log($"���[���Q���l���ύX:{_roomInfo.CurrentParticipantCount}");

    }

    /// <summary>
    /// �z�X�g�̏ꍇ�Ƀ��[�������擾
    /// </summary>
    /// <param name="roomInfo"></param>
    public void SetRoomInfo(RoomInfo roomInfo)
    {

        this._roomInfo = roomInfo;

    }

}
