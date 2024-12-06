using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// RoomManager.cs
/// クラス説明
/// ネットワーク管理
///
/// 作成日: 10/9
/// 作成者: 高橋光栄
/// </summary>
public class RoomManager : NetworkBehaviour, INetworkRunnerCallbacks
{

    [SerializeField, Tooltip("ネットワークランナー")]
    private NetworkRunner _networkRunner = default; // 静的なインスタンスとして宣言

    [SerializeField, Tooltip("ランナーが開始してあるかを描画するテキスト")]
    private Text _runnerStatusText = default;
    [SerializeField, Tooltip("プレイヤー数を描画するテキスト")]
    private Text _playerCountText = default;

    /// <summary>
    /// 部屋情報を格納するためのクラス
    /// </summary>
    private RoomInfo _preDefinedRoom = default;

    /// <summary>
    /// 参加者オブジェクトをスポーンさせる
    /// </summary>
    private IParticipantsSpawner _iSpawner = default;

    private int _memoryPlayerCount = default;



    private void Start()
    {

        GetInfo();
        Debug.Log($"NetworkObjectのアタッチの可否: {this.GetComponent<NetworkObject>() != null}");
        // 入室
        JoinRoom();

    }

    private void GetInfo()
    {

        _preDefinedRoom = this.GetComponent<RoomInfo>();
        if (_preDefinedRoom == null)
        {

            Debug.LogError("ルーム管理クラスの獲得に失敗おっぱい");

        }
        _iSpawner = this.GetComponent<IParticipantsSpawner>();
        if (_iSpawner == null)
        {

            Debug.LogError("IParticipantsSpawnerの獲得に失敗おっぱい");

        }
        // ネットワークランナーの存在確認
        if (_networkRunner == null)
        {

            print("ランナーを作成しました");
            _networkRunner = gameObject.AddComponent<NetworkRunner>();

        }

    }

    /// <summary>
    /// 部屋に入室するための処理
    /// </summary>
    /// <param name="roomName"></param>
    private async void JoinRoom()
    {
        Debug.Log($"入室を試みます。");
        if (_preDefinedRoom == null)
        {

            Debug.LogError($"部屋が見つかりません。");
            return;

        }
        // 参加者を生成
        if (await _iSpawner.Spawner( _networkRunner, _preDefinedRoom))
        {

            print("生成成功");


        }
        else
        {

            print("生成失敗");

        }

    }
    


    // 固定メソッド
    #region
    // INetworkRunnerCallbacks を実装するために必要なメソッド群

    /// <summary>
    /// プレイヤーが接続をリクエストしたときに呼ばれる
    /// </summary>
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // ここで接続リクエストに応じる処理を行う（必要であれば）
        Debug.Log("ConnectRequest received");
    }

    /// <summary>
    /// カスタム認証のレスポンスが来たときに呼ばれる
    /// </summary>
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // カスタム認証の結果を処理する（認証が必要な場合）
        Debug.Log("CustomAuthenticationResponse received");
    }

    /// <summary>
    /// 信頼性のあるデータが受信されたときに呼ばれる
    /// </summary>
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        // データを処理する（信頼性のあるデータ受信時）
        Debug.Log("Reliable data received");
    }

    /// <summary>
    /// セッションリストが更新されたときに呼ばれる
    /// </summary>
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // セッションリストが更新された際の処理（必要であればUIなどに反映）
        Debug.Log("Session list updated");
    }

    /// <summary>
    /// ユーザーシミュレーションメッセージが送られてきたときに呼ばれる
    /// </summary>
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // シミュレーションメッセージを処理する
        Debug.Log("User simulation message received");
    }

    /// <summary>
    /// 接続が失敗したときに呼ばれる
    /// </summary>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} has joined.");
    }

    /// <summary>
    /// プレイヤーが切断されたときに呼ばれる
    /// </summary>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} has left.");
    }

    /// <summary>
    /// ゲームがスタートしたときに呼ばれる
    /// </summary>
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // プレイヤーからの入力を処理する
    }

    /// <summary>
    /// ネットワークの状態が変更されたときに呼ばれる
    /// </summary>
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"ネットワークの状態が変更されました {shutdownReason}");
    }

    /// <summary>
    /// シーンがロードされたときに呼ばれる
    /// </summary>
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load done");
    }

    /// <summary>
    /// シーンロードが開始されたときに呼ばれる
    /// </summary>
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene load started");
    }

    /// <summary>
    /// ネットワークのイベントが処理されたときに呼ばれる
    /// </summary>
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.LogWarning($"Input missing for player {player.PlayerId}");
    }

    /// <summary>
    /// ネットワーク接続に問題が発生したときに呼ばれる
    /// </summary>
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server");
    }

    /// <summary>
    /// 接続が成功したときに呼ばれる
    /// </summary>
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }

    /// <summary>
    /// プレイヤーがドロップしたときに呼ばれる
    /// </summary>
    public void OnPlayerFailedToJoin(NetworkRunner runner, PlayerRef player, StartGameResult startGameResult)
    {
        Debug.LogError($"Player {player.PlayerId} failed to join: {startGameResult.ShutdownReason}");
    }

    /// <summary>
    /// ホストが移行されたときに呼ばれる
    /// </summary>
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host migration occurred");
    }

    /// <summary>
    /// 接続が失敗したときに呼ばれる
    /// </summary>
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"Failed to connect to {remoteAddress} due to {reason}");
    }

    #endregion


}


