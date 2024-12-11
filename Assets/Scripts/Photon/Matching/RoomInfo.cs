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

    [Networked] public string RoomName { get; set; } = "RoomTest";
    // ���݂̃v���C���[��[�l�b�g���[�N��œ���]
    [Networked] public int CurrentParticipantCount { get; set; } = 0 ;
    // �ő�v���C���[��[�l�b�g���[�N��œ���]
    [Networked] public int MaxParticipantCount { get; set; } = 4;

}