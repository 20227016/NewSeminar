using Fusion;

/// <summary>
/// ネットワークで使用する変数の管理
/// 現在の役割
/// ・RoomInfoで現在の部屋の人数をCurrentPlayerCountに代入する(同期変数)
/// </summary>
public class RoomInfo
{

    [Networked] public string RoomName { get; private set; }
    [Networked] public int CurrentPlayerCount { get; set; } // 現在のプレイヤー数[ネットワーク上で同期]
    [Networked] public int MaxPlayerCount { get; private set; } // 最大プレイヤー数[ネットワーク上で同期]

    public RoomInfo(string roomName, int maxPlayerCount)
    {
        RoomName = roomName;
        MaxPlayerCount = maxPlayerCount;
        CurrentPlayerCount = 0; // 初期人数
    }

    public void UpdatePlayerCount(int newPlayerCount)
    {
        CurrentPlayerCount = newPlayerCount; // プレイヤー数を更新する
    }
}