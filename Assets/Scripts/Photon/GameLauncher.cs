using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    public static GameLauncher _instance = default;

    [SerializeField, Tooltip("ネットワークランナー")]
    private NetworkRunner networkRunnerPrefab = default;

    [SerializeField, Tooltip("プレイヤーアバター")]
    private NetworkPrefabRef _participantsPrefab = default;

    [SerializeField, Tooltip("プレイヤーのスポーン位置")]
    private Vector3 _playerSpawnPos = default;

    /// <summary>
    /// 接続をホストに解除された参加者
    /// </summary>
    private List<PlayerRef> _disconnectPlayerRef = new ();

    /// <summary>
    /// 部屋管理
    /// </summary>
    private RoomInfo _roomInfo = default;

    /// <summary>
    /// ホストに
    /// </summary>
    private  IRoomController _iRoomController = default;

    /// <summary>
    /// メインカメラ
    /// </summary>
    private Camera _mainCamera = default;

    /// <summary>
    /// インプットシステム
    /// </summary>
    private PlayerInput _playerInput = default;
    
    /// <summary>
    /// ネットワークランナー
    /// </summary>
    NetworkRunner _networkRunner = default;

    /// <summary>
    /// インプットステート
    /// </summary>
    private readonly Dictionary<string, bool> InputState = new()
    {
        { "Run", false },
        { "AttackLight", false },
        { "AttackStrong", false },
        { "Targetting", false },
        { "Skill", false },
        { "Resurrection", false },
        { "Avoidance", false }
    };

    /// <summary>
    /// 移動値
    /// </summary>
    private Vector2 _moveInput = default;

    public static GameLauncher Instance
    {
        get
        {
            return _instance;
        }
    }

    /// <summary>
    /// 開始前処理
    /// </summary>
    private async void Awake()
    {

        Debug.Log($"Awake処理");
        // インスタンスがあるかつ自分ではないとき
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        // ロード時に消さない
        DontDestroyOnLoad(this.gameObject);
        /*　　　取得　　　*/
        _mainCamera = Camera.main;
        _playerInput = this.GetComponent<PlayerInput>();
        _networkRunner = Instantiate(networkRunnerPrefab);
        // 関数登録
        RegisterInputActions(true);
        // コールバック設定
        _networkRunner.AddCallbacks(this);
        StartGameArgs startGameArgs = new StartGameArgs
        {
            // ゲームモード
            GameMode = GameMode.AutoHostOrClient,
            // セッション名
            SessionName = "Room",
            // ネットワーク上でのシーン遷移同期？
            SceneManager = this.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            // セッション作成時に、現在のシーンに置かれたシーンオブジェクトをスポーンする
            Scene = SceneManager.GetActiveScene().buildIndex,
        };
        // ランナースタート
        StartGameResult result = await _networkRunner.StartGame(startGameArgs);
        Debug.Log($"サーバー状態:{_networkRunner.IsServer}");
        if (_networkRunner.IsServer)
        {

            Debug.Log($"ホスト　で{startGameArgs}の設定通りにセッション開始");

        }
        else
        {

            Debug.Log($"クライアント　で{startGameArgs}の設定通りにセッション開始");

        }
        

    }

    /// <summary>
    /// 関数登録
    /// </summary>
    /// <param name="isRegister"></param>
    private void RegisterInputActions(bool isRegister)
    {
        // 一括登録
        if (isRegister)
        {
            foreach (InputAction action in _playerInput.actions)
            {
                action.performed += HandleInput;
                action.canceled += HandleInput;
            }
        }
        // 一括解除
        else
        {
            foreach (InputAction action in _playerInput.actions)
            {
                action.performed -= HandleInput;
                action.canceled -= HandleInput;
            }
        }
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {

        _roomInfo = GameObject.Find("Room").GetComponent<RoomInfo>();
        if (_roomInfo == null)
        {

            Debug.LogError("ルーム管理情報が入っていません");

        }

    }

    /// <summary>
    /// 入力管理メソッド
    /// </summary>
    /// <param name="context">入力アクション</param>
    private void HandleInput(InputAction.CallbackContext context)
    {

        switch (context.action.name)
        {

            case "Move":
                _moveInput = context.ReadValue<Vector2>();
                break;

            case "Run":
                InputState["Run"] = true;
                break;

            case "AttackLight":
                InputState["AttackLight"] = !context.canceled;
                break;

            case "AttackStrong":
                InputState["AttackStrong"] = !context.canceled;
                break;

            case "Targetting":
                InputState["Targetting"] = !context.canceled;
                break;

            case "Skill":
                InputState["Skill"] = !context.canceled;
                break;

            case "Resurrection":
                InputState["Resurrection"] = !context.canceled;
                break;

            case "Avoidance":
                InputState["Avoidance"] = !context.canceled;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// カメラの向きを基準にした移動方向算出メソッド
    /// </summary>
    /// <returns>カメラの向きを基準にした移動方向</returns>
    private Vector3 GetMoveDirectionFromCamera()
    {
        // カメラの正面方向と右方向を取得
        Vector3 cameraForward = _mainCamera.transform.forward;
        Vector3 cameraRight = _mainCamera.transform.right;

        // 高さ（Y軸）の影響を排除
        cameraForward.y = 0;
        cameraRight.y = 0;

        // 正規化
        cameraForward.Normalize();
        cameraRight.Normalize();

        // カメラ基準での移動方向を計算
        Vector3 direction = cameraForward * _moveInput.y + cameraRight * _moveInput.x;
        return new Vector2(direction.x, direction.z);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // 入力をインスタンス化
        PlayerNetworkInput data = new()
        {
            MoveDirection = GetMoveDirectionFromCamera(),
            IsRunning = InputState["Run"],
            IsAttackLight = InputState["AttackLight"],
            IsAttackStrong = InputState["AttackStrong"],
            IsTargetting = InputState["Targetting"],
            IsSkill = InputState["Skill"],
            IsResurrection = InputState["Resurrection"],
            IsAvoidance = InputState["Avoidance"],
        };

        // 入力を収集
        input.Set(data);

        // 入力を初期化
        ResetInput();
    }

    /// <summary>
    /// 入力初期化メソッド
    /// </summary>
    private void ResetInput()
    {
        // trueの入力を初期化する
        foreach (string key in new List<string>(InputState.Keys))
        {

            if (InputState[key])
            {
                InputState[key] = false; // 値をリセット
            }
        }
    }

    /// <summary>
    /// プレイヤーが参加した時の処理
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="player"></param>
    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        Debug.Log($"プレイヤー参加");
        Debug.Log($"{_roomInfo.CurrentParticipantCount }");
        // ホストで参加するとき
        if (!runner.IsServer)
        {
            return;
        }
        Vector3 spawnPosition = new Vector3(_playerSpawnPos.x + UnityEngine.Random.Range(0, 10), _playerSpawnPos.y, _playerSpawnPos.z);
        NetworkObject participantsObj = default;
        if (_roomInfo.CurrentParticipantCount == 0)
        {

            Debug.Log($"ホストオブジェクトを生成");
            participantsObj = runner.Spawn(_participantsPrefab, spawnPosition, Quaternion.identity, runner.LocalPlayer);
            participantsObj.name = $"{_roomInfo.CurrentParticipantCount}_Host";
            _iRoomController = participantsObj.GetComponent<IRoomController>();
            if (_iRoomController == null)
            {

                Debug.LogError("ホストオブジェクトにルーム情報を渡すインターフェースが見つかりません");

            }

        }
        else
        {

            if(_roomInfo.CurrentParticipantCount >= _roomInfo.MaxParticipantCount)
            {

                Debug.Log($"最大人数の{_roomInfo.MaxParticipantCount}人に達したため参加できません");
                Debug.Log($"セッションを終了します");
                runner.Disconnect(player);
                _disconnectPlayerRef.Add(player);
                return;

            }
            Debug.Log($"クライアントオブジェクトを生成");
            participantsObj = runner.Spawn(_participantsPrefab, spawnPosition, Quaternion.identity);
            participantsObj.name = $"{_roomInfo.CurrentParticipantCount}_Client";
        
        }
        runner.SetPlayerObject(player, participantsObj);
        // 参加人数を増やす
        _iRoomController.RPC_ParticipantCountAdd();

    }

    /// <summary>
    /// プレイヤーが退出した時の処理
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="player"></param>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
        {
            return;
        }
        // ホストに切断されたことがセッションを終了原因の場合
        if (_disconnectPlayerRef.Contains(player))
        {

            _disconnectPlayerRef.Remove(player);
            return;

        }
        if (runner.TryGetPlayerObject(player, out NetworkObject avatar))
        {
            runner.Despawn(avatar);
        }
        // 参加人数を減らす
        _iRoomController.RPC_ParticipantCountRemove();

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
