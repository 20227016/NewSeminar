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
    /// �J�n����
    /// </summary>
    void Start()
    {

        // �Q���l�������Z
        RPC_ParticipantCountAdd();
        _netObj = this.GetComponent<NetworkObject>();

    }

    
    /// <summary>
    /// �Q���Ґl�������Z
    /// �Ăяo��:�z�X�g�ƃN���C�A���g
    /// ���s����:�z�X�g
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void RPC_ParticipantCountAdd()
    {

        await WaitForNotNull();
        _roomInfo.CurrentParticipantCount++;

    }

    /// <summary>
    ///  ���g������܂őҋ@
    /// </summary>
    /// <returns></returns>
    private async Task WaitForNotNull()
    {

        while (_roomInfo == null)
        {

            // _roomInfo �� null �̏ꍇ�� 100�~���b�ҋ@���čĊm�F
            await Task.Delay(100);

        }

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
