using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// �l�b�g���[�N�Ŏg�p����ϐ��̊Ǘ�
/// ���݂̖���
/// �ERoomInfo�Ō��݂̕����̐l����CurrentPlayerCount�ɑ������(�����ϐ�)
/// </summary>
public class RoomInfo : NetworkBehaviour
{

    [Networked] public string RoomName { get => _roomName; set => _roomName = value; }
    // ���݂̃v���C���[��[�l�b�g���[�N��œ���]
    [Networked] public int CurrentParticipantCount { get => _currentParticipantCount; set => _currentParticipantCount = value; }
    // �ő�v���C���[��[�l�b�g���[�N��œ���]
    [Networked] public int MaxParticipantCount { get => _maxParticipantCount; set => _maxParticipantCount = value; }

    private string _roomName = "Room";
    private int _currentParticipantCount = 0;
    private int _maxParticipantCount = 4;

    private NetworkObject _networkObject = default;


    public void SetRoomInfo(string roomName, int maxPlayerCount)
    {
        RoomName = roomName;
        MaxParticipantCount = maxPlayerCount;
        CurrentParticipantCount = 0; // �����l��
    }

}