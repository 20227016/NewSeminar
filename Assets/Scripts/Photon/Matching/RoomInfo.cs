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

    [Networked] public string RoomName { get; set; } = "RoomTest";
    // 現在のプレイヤー数[ネットワーク上で同期]
    [Networked] public int CurrentParticipantCount { get; set; } = 0 ;
    // 最大プレイヤー数[ネットワーク上で同期]
    [Networked] public int MaxParticipantCount { get; set; } = 4;

}