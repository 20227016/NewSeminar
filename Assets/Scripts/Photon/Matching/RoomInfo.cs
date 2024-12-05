using Fusion;

/// <summary>
/// ネットワークで使用する変数の管理
/// 現在の役割
/// ・RoomInfoで現在の部屋の人数をCurrentPlayerCountに代入する(同期変数)
/// </summary>
public class RoomInfo
{

    [Networked] public string RoomName { get; private set; }
    [Networked] public int CurrentParticipantCount { get; set; } // 現在のプレイヤー数[ネットワーク上で同期]
    [Networked] public int MaxParticipantCount { get; private set; } // 最大プレイヤー数[ネットワーク上で同期]

    public RoomInfo(string roomName, int maxPlayerCount)
    {
        RoomName = roomName;
        MaxParticipantCount = maxPlayerCount;
        CurrentParticipantCount = 0; // 初期人数
    }

    public void UpdateParticipantCount(int newParticipantCount)
    {
        CurrentParticipantCount = newParticipantCount; // プレイヤー数を更新する
    }
}