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

    [SerializeField, Tooltip("部屋参加ボタンのプレハブ")]
    private GameObject _roomButtonPrefab = default; 

    [SerializeField, Tooltip("ページスライドするやつ")]
    private Transform _roomListContent = default; // ScrollView の Content にアタッチ

    [SerializeField, Tooltip("ランナーが開始してあるかを描画するテキスト")]
    private Text _runnerStatusText = default;
    [SerializeField, Tooltip("プレイヤー数を描画するテキスト")]
    private Text _playerCountText = default;

    private Text _buttonText = default;

    private StartGameResult _result = default;

    private IParticipantsSpawner _iSpawner = default;
   

    /// <summary>
    /// 現在の参加人数
    /// </summary>
    private int currentPlayerCount = 0;


    /// <summary>
    ///  部屋情報を格納するためのシンプルなリスト
    /// </summary>
    private List<RoomInfo> _preDefinedRooms = new List<RoomInfo>
    {
         new RoomInfo("Room 1", 4),
         new RoomInfo("Room 2", 4),
         new RoomInfo("Room 3", 4),
         new RoomInfo("Room 4", 4),
    };

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
        // UIを初期化
        InitializeRoomListUI();

    }

    /// <summary>
    /// 初期処理
    /// </summary>
    private void InitializeRoomListUI()
    {

        // リストにあるストリング型(roomNames)を取得し、それをroomNameに代入する(リストにある命名数分、ループし、ボタンを生成する)
        foreach (RoomInfo room in _preDefinedRooms)
        {

            // ボタンのプレハブをインスタンス化
            GameObject roomButtonInstance = Instantiate(_roomButtonPrefab, _roomListContent);

            // ボタンにルーム名を表示
            _buttonText = roomButtonInstance.GetComponentInChildren<Text>();
            if (_buttonText != null)
            {
                _buttonText.text = room.RoomName;
            }

            // ボタンをクリックしたら、JoinRoom(メソッド)を呼ぶ
            Button button = roomButtonInstance.GetComponent<Button>();
            if (button != null)
            {
                // クリックしたルームの名前(roomName)を取得し、JoinRoom(string roomName)に送る
                button.onClick.AddListener(() => JoinRoom(room.RoomName));
            }
        }
    }


    /// <summary>
    /// 部屋に入室するための処理
    /// </summary>
    /// <param name="roomName"></param>
    private async void JoinRoom(string roomName)
    {
        Debug.Log($"{roomName} に入室を試みます。");
        // リストと比べて一致するRoomを抜き出す
        RoomInfo room = _preDefinedRooms.Find(r => r.RoomName == roomName);
        if (room == null)
        {
            Debug.LogError($"部屋 {roomName} が見つかりません。");
            return;
        }

        // プレイヤー数が0ならホスト、それ以外はクライアント
        GameMode gameMode = room.CurrentPlayerCount == 0 ? GameMode.Host : GameMode.Client;

        // サーバー情報（Args＝引数）
        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = gameMode,
            SessionName = roomName,
            SceneManager = this.gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        // ゲームモードに合わせた参加者を生成
        if (await _iSpawner.Spawner(startGameArgs,_networkRunner))
        {

            print("接続成功");

        }
        else
        {

            print("接続失敗");

        }

        //// 指定したネットワークモードでのゲームをスタートさせる（セッション名が重複している等で失敗する）
        //StartGameResult result = await _networkRunner.StartGame(startGameArgs);
        //if (result.Ok)
        //{
        //    Debug.Log($"部屋 {roomName} に正常に参加しました。");

        //    if (gameMode == GameMode.Host)
        //    {

        //        // ホストがStateAuthorityを持つために、ネットワークオブジェクトのStateAuthorityをホストに設定
        //        NetworkObject networkObject = GetComponent<NetworkObject>();

        //        // 必要な場合、StateAuthorityをホストに設定
        //        if (networkObject != null && !networkObject.HasStateAuthority)
        //        {
        //            // 自分のPlayerに対してStateAuthorityを設定
        //            //networkObject.AssignStateAuthority(_networkRunner.LocalPlayer);
        //        }

        //        Debug.Log("ホストとして部屋に参加しました。");
        //        UpdatePlayerCount(roomName, 1); // ホストの初期設定
        //    }
        //    else
        //    {
        //        Debug.Log("クライアントとして部屋に参加しました。");
        //        // 参加リクエスト（プレイヤー数をホストに通知）
        //        //RPC_RequestPlayerCountUpdate(roomName);
        //    }
        //}
        //else
        //{
        //    print(gameMode);
        //    print(result);
        //    Debug.LogError($"部屋 {roomName} に参加できませんでした: {result.ShutdownReason}");
        //}

        //// 最新のプレイヤー数を確認
        //Debug.Log($"部屋 {roomName} の現在のプレイヤー数: {room.CurrentPlayerCount}");

    }

    private void Update()
    {
        _runnerStatusText.text = "ランナー " + _networkRunner.IsRunning;
        _playerCountText.text = "現在の人数 " + currentPlayerCount;
    }

    // -------------------------------------------------------------------------------クライアントがホストからプレイヤー数を取得するための総合処理

    private async Task<int> GetPlayerCountFromHost(string roomName)
    {
        // RPCでホストからプレイヤー数を取得する処理
        return await Task.FromResult(0); // 仮の処理。実際にはRPCを実装
    }
    // クライアントにプレイヤー数を送信するRPC
    private void RPC_SendPlayerCount(int playerCount)
    {
        // プレイヤー数をクライアント側で処理する
        // 例: playerCountを保存またはUIを更新
        Debug.Log($"プレイヤー数: {playerCount}");
    }

    // ---------------------------------------------------------------------------------------終わり



    /// <summary>
    /// プレイヤーが入室したことを他のクライアントに通知するメソッド
    /// RPCの発信者を指定(RpcSources.StateAuthorit)し、
    /// 全クライアント(RpcTargets.All)に通知を送る
    /// 〇このメソッドが呼ばれるタイミング
    /// ・プレイヤーが部屋に参加したとき
    /// </summary>
    /// <param name="roomName">部屋の変数名</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyPlayerJoined(string roomName, int currentPlayerCount)
    {

        Debug.Log($"RPCで通知: 部屋 {roomName} にプレイヤーが参加しました");

        // 既存のUIを更新するためのメソッド
        // 入室した部屋のプレイヤー数を更新
        UpdateRoomInfo(roomName, currentPlayerCount);
    }

    /// <summary>
    /// 全てのクライアントで部屋の情報を更新する(今のところは人数の更新)
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="currentPlayerCount"></param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateRoomInfo(string roomName, int newPlayerCount)
    {

        RoomInfo room = _preDefinedRooms.Find(r => r.RoomName == roomName);
        if (room != null)
        {
            // 現在のnewPlayerCountをCurrentPlayerCountに代入する
            room.UpdatePlayerCount(newPlayerCount);
            Debug.Log($"部屋 {roomName} の人数を更新: {newPlayerCount}");
        }
    }

    /// <summary>
    /// 部屋の情報を更新する
    /// </summary>
    private void UpdateRoomInfo(string roomName, int newPlayerCount)
    {
        foreach (RoomInfo room in _preDefinedRooms)
        {
            if (room.RoomName == roomName)
            {
                //// プレイヤー数を更新
                //room.CurrentPlayerCount = newPlayerCount;

                //// ボタンのテキストを更新
                //_buttonText = room.ButtonInstance.GetComponentInChildren<TextMeshProUGUI>();
                //if (_buttonText != null)
                //{
                //    _buttonText.text = $"{room.RoomName} ({room.CurrentPlayerCount}/{room.MaxPlayerCount})";
                //}
                break;
            }
        }
    }



    // 部屋名を渡して、現在のプレイヤー数を取得するメソッド
    private int GetCurrentPlayerCount(string roomName)
    {
        // 指定された部屋名に基づいて RoomInfo を検索
        RoomInfo room = _preDefinedRooms.Find(r => r.RoomName == roomName);
        if (room != null)
        {

            Debug.Log("人数は"+ room.CurrentPlayerCount + "です");
            // 部屋が見つかった場合、その部屋の現在のプレイヤー数を返す
            return room.CurrentPlayerCount;
        }
        Debug.Log("ルームが見つからない");
        // 部屋が見つからなかった場合、0を返す（エラー処理も検討可能）
        return 0;
    }

    // ホストが人数を更新する処理
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void UpdatePlayerCount(string roomName, int newPlayerCount)
    {
        print("送られてきた部屋名"+roomName);
        print("送られてきた人数(1が理想)"+newPlayerCount);
        RoomInfo room = _preDefinedRooms.Find(r => r.RoomName == roomName);
        if (room != null)
        {
            print("人数の更新処理を開始します");
            room.UpdatePlayerCount(newPlayerCount);
            RPC_UpdateRoomInfo(roomName, newPlayerCount); // 全クライアントに同期
            print("UpdatePlayerCountで更新処理が呼び出されました。現在のプレイヤー数は "+ newPlayerCount);
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