using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField, Tooltip("ネットワークランナープレハブ")]
    private NetworkRunner _networkRunnerPrefab = default;

    [SerializeField, Tooltip("コンボカウンタープレハブ")]
    private NetworkPrefabRef _comboCounterPrefab = default;

    [SerializeField, Tooltip("プレイヤープレハブ")]
    private NetworkPrefabRef _playerPrefab = default;

    [SerializeField, Tooltip("プレイヤーのスポーン位置")]
    private Vector3 _playerSpawnPos = default;

    // インプットシステム
    private PlayerInput _playerInput = default;

    // 移動入力方向
    private Vector2 _moveInput = default;

    // メインカメラ
    private Camera _mainCamera = default;

    [SerializeField]
    private NetworkObject _portal = default;

    public Vector3 _portalPosition = new Vector3(-10f, -1.35f, 0f);

    private NetworkRunner _networkRunner = default;

    private Subject<Unit> _startGameSubject = new();

    [SerializeField]
    private EnemySpawner _enemySpawner = default;

    private string _sessionName = default;

    /// <summary>
    /// スポーンしたかのフラグ
    /// </summary>
    private bool _hasSpawn = default;

    public Subject<Unit> StartGameSubject { get => _startGameSubject; set => _startGameSubject = value; }

    public NetworkRunner NetworkRunner { get => _networkRunner; set => _networkRunner = value; }
    public string SessionName { get => _sessionName; set => _sessionName = value; }

    private bool _isGameClear = false;

    private void Awake()
    {
        
        var existingRunners = FindObjectsOfType<NetworkSceneManagerDefault>();
        foreach (var runner in existingRunners)
        {
            Destroy(runner.gameObject);
        }
    }

    private async void Start()
    {
        _enemySpawner.OnGameClearObservable
            .Subscribe(_ => _isGameClear = true)
            .AddTo(this);

        _playerInput = GetComponent<PlayerInput>();
        RegisterInputActions(true);

        _mainCamera = Camera.main;

        // 既存のNetworkRunnerを探す
        NetworkRunner existingRunner = FindObjectOfType<NetworkRunner>();

        if (existingRunner != null)
        {
            NetworkRunner = existingRunner;
        }
        else
        {
            NetworkRunner = Instantiate(_networkRunnerPrefab);
            NetworkRunner.AddCallbacks(this);
        }

        SessionManager sessionManager = FindObjectOfType<SessionManager>();
        
        if (sessionManager != null)
        {
            if(sessionManager.SessionName == null)
            {
                SessionName = "あ";
            }
            else
            {
                SessionName = sessionManager.SessionName;
            }
        }
        else
        {
            SessionName = UnityEngine.Random.Range(10000, 99999).ToString();
        }

        // ゲーム開始処理
        await NetworkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = SessionName,
            SceneManager = NetworkRunner.GetComponent<NetworkSceneManagerDefault>()
        });

        _startGameSubject.OnNext(Unit.Default);
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

    private readonly Dictionary<string, bool> InputStates = new()
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

    /// <summary>
    /// プレイヤーが参加したときの処理
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="player"></param>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
        {
            return;
        }
        if (!_hasSpawn)
        {

            _hasSpawn = true;
            // ここでComboCounterを生成
            if (runner.IsServer)
            {
                InitialSpaen(runner);
            }

        }
        // 現在のプレイヤー数を確認
        if (runner.ActivePlayers.Count() > 4) // 最大4人まで
        {
            runner.Disconnect(player);  // ネットワークから切断
            return;
        }

        Vector3 spawnPosition = new Vector3(_playerSpawnPos.x + UnityEngine.Random.Range(0, 10), _playerSpawnPos.y, _playerSpawnPos.z);

        // プレイヤーを生成
        NetworkObject playerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

        runner.SetPlayerObject(player, playerObject);
    }

    // ComboCounterを生成するメソッド
    private void InitialSpaen(NetworkRunner runner)
    {
        runner.Spawn(_comboCounterPrefab, Vector3.zero, Quaternion.identity);
        runner.Spawn(_portal, _portalPosition, Quaternion.Euler(90, 0, 0));

        StartCoroutine(GameOverStart());

    }

    private IEnumerator GameOverStart()
    {
        yield return new WaitForSeconds(2); // 2秒待つ
        _enemySpawner.GameOverStart();
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

        if (runner.TryGetPlayerObject(player, out var avatar))
        {
            runner.Despawn(avatar);
        }

        PlayerUIPresenter playerUIPresenter = FindObjectOfType<PlayerUIPresenter>();

        playerUIPresenter.ClearAllyModels();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
    {
        Debug.Log($"シャットダウン理由: {shutdownReason}");

        runner.Shutdown();
        Destroy(runner);
        Debug.Log(_isGameClear);
        if (_isGameClear)
        {
            SceneManager.LoadScene("GameClear");
        }
        else
        {
            SceneManager.LoadScene("Title");
        }
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) 
    {
        Destroy(runner);
        if (_isGameClear)
        {
            SceneManager.LoadScene("GameClear");
        }
        else
        {
            SceneManager.LoadScene("Title");
        }
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) {  }
    public void OnSceneLoadDone(NetworkRunner runner) {  }
}
