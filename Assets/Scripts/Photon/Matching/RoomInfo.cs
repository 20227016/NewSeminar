using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// ネットワークで使用する変数の管理
/// 現在の役割
/// ・RoomInfoで現在の部屋の人数をCurrentPlayerCountに代入する(同期変数)
/// </summary>
public class RoomInfo : NetworkBehaviour
{

    [Networked] public string RoomName { get => _roomName; set => _roomName = value; }
    // 現在のプレイヤー数[ネットワーク上で同期]
    [Networked] public int CurrentParticipantCount { get => _currentParticipantCount; set => _currentParticipantCount = value; }
    // 最大プレイヤー数[ネットワーク上で同期]
    [Networked] public int MaxParticipantCount { get => _maxParticipantCount; set => _maxParticipantCount = value; }

    private string _roomName = "Room";
    private int _currentParticipantCount = 0;
    private int _maxParticipantCount = 4;

    private NetworkObject _networkObject = default;


    public void SetRoomInfo(string roomName, int maxPlayerCount)
    {
        RoomName = roomName;
        MaxParticipantCount = maxPlayerCount;
        CurrentParticipantCount = 0; // 初期人数
    }

}