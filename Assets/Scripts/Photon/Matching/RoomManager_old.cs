using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// RoomManager.cs
/// クラス説明
/// マッチング機能
///
/// 作成日: /
/// 作成者: 
/// </summary>
public class RoomManager_old : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner _networkRunner;

    private Dictionary<string, SessionInfo> availableRooms = new Dictionary<string, SessionInfo>();

    public event Action<List<SessionInfo>> OnSessionListUpdatedCallback;

    List<SessionInfo> sessionList; 

    private void Start()
    {
        _networkRunner.AddCallbacks(this);
    }

    // ルームの作成
    public async void CreateRoom(string roomName)
    {
        print("部屋作成");
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Host,    // ホストとしてゲームを開始
            SessionName = roomName,      // ルーム名を設定
            PlayerCount = 4,             // 最大プレイヤー数は4人
            Scene = SceneManager.GetActiveScene().buildIndex, // シーン情報も必要
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };
        await _networkRunner.StartGame(startGameArgs);
    }

    // ルーム検索
    public void SearchRooms()
    {
       
        if (_networkRunner != null && !_networkRunner.IsRunning)
        {
            print("部屋検索");
            // ルームの検索を開始
            _networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client
            });
        }
    }

    // 任意のルームに参加
    public async void JoinRoom(string roomName)
    {
        if (availableRooms.ContainsKey(roomName))
        {
            var joinArgs = new StartGameArgs()
            {
                GameMode = GameMode.Client, // クライアントとして参加
                SessionName = roomName,     // 参加するルーム名を指定
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            await _networkRunner.StartGame(joinArgs);
        }
        else
        {
            Debug.LogWarning($"Room {roomName} not found.");
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("部屋リスト更新");

        // コールバックを発行してUI更新を通知
        OnSessionListUpdatedCallback?.Invoke(sessionList);

        // デバッグ用に取得した部屋情報を出力
        foreach (var session in sessionList)
        {
            Debug.Log($"Room: {session.Name}, Players: {session.PlayerCount}/4");
        }
    }

    // 必要なコールバック関数（空実装）
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}