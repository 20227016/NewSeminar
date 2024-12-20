using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameLauncher : NetworkBehaviour, INetworkRunnerCallbacks
{

    /// <summary>
    /// シングルトン
    /// </summary>
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
    private List<PlayerRef> _disconnectPlayerRef = new();

    /// <summary>
    /// 部屋管理
    /// </summary>
    private RoomInfo _roomInfo = default;

    /// <summary>
    /// ホストに
    /// </summary>
    private IRoomController _iRoomController = default;

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
    private NetworkRunner _networkRunner = default;

    private bool _isMySpawned = false;

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

    /// <summary>
    /// シングルトン
    /// </summary>
    public static GameLauncher Instance
    {
        get
        {
            return _instance;
        }
    }

    private async void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        Debug.Log($"Awake処理＿開始: {this.GetType().Name}クラス");
        /*　　　取得　　　*/
        _playerInput = this.GetComponent<PlayerInput>();
        _networkRunner = Instantiate(networkRunnerPrefab);
        // 関数登録
        RegisterInputActions(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        // コールバック設定
        _networkRunner.AddCallbacks(this);
        // セッション設定
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
        Debug.LogError($"サーバー状態:{_networkRunner.IsRunning}");
        if (_networkRunner.IsServer)
        {

        NetworkRunner networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.AddCallbacks(this);

        StartGameResult result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>()
        });
        // ここでComboCounterを生成
        if (networkRunner.IsServer)
        {
            SpawnComboCounter(networkRunner);
        }
    // ComboCounterを生成するメソッド
    private void SpawnComboCounter(NetworkRunner runner)
    {
        // プレイヤーの位置などを指定してComboCounterを生成します
        Vector3 spawnPosition = new Vector3(0, 0, 0); // 必要に応じて調整
        runner.Spawn(comboCounterPrefab, spawnPosition, Quaternion.identity);
    }

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
                InputStates["Run"] = true;
                break;

            case "AttackLight":
                InputStates["AttackLight"] = !context.canceled;
                break;

            case "AttackStrong":
                InputStates["AttackStrong"] = !context.canceled;
                break;

            case "Targetting":
                InputStates["Targetting"] = !context.canceled;
                break;

            case "Skill":
                InputStates["Skill"] = !context.canceled;
                break;

            case "Resurrection":
                InputStates["Resurrection"] = !context.canceled;
                break;

            case "Avoidance":
                InputStates["Avoidance"] = !context.canceled;
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
            IsRunning = InputStates["Run"],
            IsAttackLight = InputStates["AttackLight"],
            IsAttackStrong = InputStates["AttackStrong"],
            IsTargetting = InputStates["Targetting"],
            IsSkill = InputStates["Skill"],
            IsResurrection = InputStates["Resurrection"],
            IsAvoidance = InputStates["Avoidance"],
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
        foreach (string key in new List<string>(InputStates.Keys))
        {

            if (InputStates[key])
            {
                InputStates[key] = false; // 値をリセット
            }
        }
    }

    // プレイヤーが参加した時の処理
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
        {
            return;
        }

        var spawnPosition = new Vector3(_playerSpawnPos.x + UnityEngine.Random.Range(0, 10), _playerSpawnPos.y, _playerSpawnPos.z);
        var avatar = runner.Spawn(playerAvatarPrefab, spawnPosition, Quaternion.identity, player);
        runner.SetPlayerObject(player, avatar);
    }

    // プレイヤーが退出した時の処理
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
        {
            return;
        }

        if (runner.TryGetPlayerObject(player, out var avatar))
        {
            runner.Despawn(avatar);
        }
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

}
