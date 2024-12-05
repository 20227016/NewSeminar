using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
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

   [SerializeField, Tooltip("部屋情報を格納するためのクラス")]
    private RoomInfo _preDefinedRoom = new RoomInfo("Room", 4);

    /// <summary>
    /// 参加者オブジェクトをスポーンさせる
    /// </summary>
    private IParticipantsSpawner _iSpawner = default;

  

    private void Start()
    {

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
        Debug.Log($"RoomManagerのStateAuthority: {this.HasStateAuthority}");
        Debug.Log($"NetworkObjectがアタッチされているか: {this.GetComponent<NetworkObject>() != null}");
        // 入室
        JoinRoom();

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
        // プレイヤー数が0ならホスト、それ以外はクライアント
        GameMode gameMode = _preDefinedRoom.CurrentParticipantCount == 0 ? GameMode.Host : GameMode.Client;
        // サーバー情報（Args＝引数）
        StartGameArgs startGameArgs = new StartGameArgs()
        {

            GameMode = gameMode,
            SessionName = _preDefinedRoom.RoomName,
            SceneManager = this.gameObject.AddComponent<NetworkSceneManagerDefault>()

        };
        // ゲームモードに合わせた参加者を生成
        if (await _iSpawner.Spawner(startGameArgs,_networkRunner,_preDefinedRoom))
        {

            print("生成成功");
            //入室情報表示
            _runnerStatusText.text = "ランナー " + _networkRunner.IsRunning;
            _playerCountText.text = "現在の人数 " + _preDefinedRoom.CurrentParticipantCount;

        }
        else
        {

            print("生成失敗");

        }

    }



    ///// <summary>
    ///// プレイヤーが入室したことを他のクライアントに通知するメソッド
    ///// RPCの発信者を指定(RpcSources.StateAuthorit)し、
    ///// 全クライアント(RpcTargets.All)に通知を送る
    ///// 〇このメソッドが呼ばれるタイミング
    ///// ・プレイヤーが部屋に参加したとき
    ///// </summary>
    ///// <param name="roomName">部屋の変数名</param>
    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RPC_NotifyPlayerJoined(string roomName, int currentPlayerCount)
    //{

    //    Debug.Log($"RPCで通知: 部屋 {roomName} にプレイヤーが参加しました");

    //    // 既存のUIを更新するためのメソッド
    //    // 入室した部屋のプレイヤー数を更新
    //    UpdateRoomInfo(roomName, currentPlayerCount);
    //}

    ///// <summary>
    ///// 全てのクライアントで部屋の情報を更新する(今のところは人数の更新)
    ///// </summary>
    ///// <param name="roomName"></param>
    ///// <param name="currentPlayerCount"></param>
    //[Rpc(RpcSources.All, RpcTargets.All)]
    //private void RPC_UpdateRoomInfo(string roomName, int newPlayerCount)
    //{

    //    RoomInfo room = _preDefinedRoom.Find(r => r.RoomName == roomName);
    //    if (room != null)
    //    {
    //        // 現在のnewPlayerCountをCurrentPlayerCountに代入する
    //        room.UpdatePlayerCount(newPlayerCount);
    //        Debug.Log($"部屋 {roomName} の人数を更新: {newPlayerCount}");
    //    }
    //}

    ///// <summary>
    ///// 部屋の情報を更新する
    ///// </summary>
    //private void UpdateRoomInfo(string roomName, int newPlayerCount)
    //{
    //    foreach (RoomInfo room in _preDefinedRoom)
    //    {
    //        if (room.RoomName == roomName)
    //        {
    //            //// プレイヤー数を更新
    //            //room.CurrentPlayerCount = newPlayerCount;

    //            //// ボタンのテキストを更新
    //            //_buttonText = room.ButtonInstance.GetComponentInChildren<TextMeshProUGUI>();
    //            //if (_buttonText != null)
    //            //{
    //            //    _buttonText.text = $"{room.RoomName} ({room.CurrentPlayerCount}/{room.MaxPlayerCount})";
    //            //}
    //            break;
    //        }
    //    }
    //}



    //// 部屋名を渡して、現在のプレイヤー数を取得するメソッド
    //private int GetCurrentPlayerCount(string roomName)
    //{
    //    // 指定された部屋名に基づいて RoomInfo を検索
    //    RoomInfo room = _preDefinedRoom.Find(r => r.RoomName == roomName);
    //    if (room != null)
    //    {

    //        Debug.Log("人数は"+ room.CurrentPlayerCount + "です");
    //        // 部屋が見つかった場合、その部屋の現在のプレイヤー数を返す
    //        return room.CurrentPlayerCount;
    //    }
    //    Debug.Log("ルームが見つからない");
    //    // 部屋が見つからなかった場合、0を返す（エラー処理も検討可能）
    //    return 0;
    //}

    //// ホストが人数を更新する処理
    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //private void RPC_UpdatePlayerCount(string roomName, int newPlayerCount)
    //{
    //    print("送られてきた部屋名"+roomName);
    //    print("送られてきた人数(1が理想)"+newPlayerCount);
    //    RoomInfo room = _preDefinedRoom.Find(r => r.RoomName == roomName);
    //    if (room != null)
    //    {
    //        print("人数の更新処理を開始します");
    //        room.UpdatePlayerCount(newPlayerCount);
    //        RPC_UpdateRoomInfo(roomName, newPlayerCount); // 全クライアントに同期
    //        print("UpdatePlayerCountで更新処理が呼び出されました。現在のプレイヤー数は "+ newPlayerCount);
    //    }
    //}

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