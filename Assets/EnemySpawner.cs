using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;

[Serializable]
public class EnemyWave
{
    [Tooltip("このウェーブでスポーンするエネミーのリスト")]
    public List<NetworkObject> Enemies = new List<NetworkObject>();

    [Tooltip("このウェーブのスポーン位置リスト")]
    public List<Transform> SpawnPositions = new List<Transform>();
}

/// <summary>
/// エネミーのスポーンを管理するスクリプト
/// </summary>
public class EnemySpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField, Tooltip("ボスへ向かうテレポーター")]
    private GameObject _bossTeleporter = default;

    [SerializeField, Tooltip("ステージをわける仕切り")]
    private GameObject _wavePartition = default;

    [SerializeField, Tooltip("ステージをわける仕切り")]
    private GameObject _wavePartitionEND = default;

    [SerializeField, Tooltip("ネットワークランナープレハブ")]
    private NetworkRunner _networkRunnerPrefab = default;

    [SerializeField, Tooltip("エネミーのウェーブごとの設定")]
    private List<EnemyWave> _enemyWaves = new List<EnemyWave>();

    // スポーン済みエネミーを管理
    private List<NetworkObject> _spawnedEnemies = new List<NetworkObject>(); 

    // Waveクリア通知
    private Subject<Unit> OnEnemiesDefeated = new Subject<Unit>();
    public IObservable<Unit> OnEnemiesDefeatedObservable => OnEnemiesDefeated;

    // ノーマルステージクリア通知
    private Subject<Unit> OnAllEnemiesDefeated = new Subject<Unit>();
    public IObservable<Unit> OnAllEnemiesDefeatedObservable => OnAllEnemiesDefeated;

    // 現在のウェーブ番号
    [Networked]
    private int _currentWave { get; set; } = 0;

    /// <summary>
    /// 初期処理
    /// </summary>
    private void Start()
    {
        GameLauncher gameLauncher = FindObjectOfType<GameLauncher>();
        NetworkRunner networkRunner = gameLauncher.NetworkRunner;

        gameLauncher.StartGameSubject.Subscribe(_ => StartWave(networkRunner));

        // エネミー全滅時の通知を購読し、次のウェーブへ
        OnEnemiesDefeated.Subscribe(_ => NextWave(networkRunner));
    }

    /// <summary>
    /// 全てのエネミーの全滅確認
    /// </summary>
    private void Update()
    {
        if (_spawnedEnemies.Count > 0 && AllEnemiesHidden())
        {
            OnEnemiesDefeated.OnNext(Unit.Default);
        }
    }

    /// <summary>
    /// 指定されたウェーブのエネミーをスポーン
    /// </summary>
    private void StartWave(NetworkRunner runner)
    {
        if (_enemyWaves.Count == 0)
        {
            Debug.LogWarning("エネミーウェーブが設定されていません！");
            return;
        }

        _currentWave = 0; // 初期ウェーブをセット
        SpawnEnemies(runner, _currentWave);
    }

    /// <summary>
    /// 次のウェーブへ進む
    /// </summary>
    private void NextWave(NetworkRunner runner)
    {
        _currentWave++;



        if (_currentWave >= _enemyWaves.Count)
        {
            Debug.Log("すべてのウェーブが終了しました！");
            if (_bossTeleporter != null)
            {
                OnAllEnemiesDefeated.OnNext(Unit.Default);
                // ボステレポーターを有効化
                _bossTeleporter.SetActive(true); 
            }
            return;
        }

        Debug.Log($"ウェーブ {_currentWave + 1} 開始！");
        SpawnEnemies(runner, _currentWave);
    }

    /// <summary>
    /// Waveごとにエネミーをスポーンさせる
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="waveIndex"></param>
    private void SpawnEnemies(NetworkRunner runner, int waveIndex)
    {
        if (runner.IsServer)
        {
            _spawnedEnemies.Clear();

            // 今のウェーブデータ
            EnemyWave wave = _enemyWaves[waveIndex];
            List<NetworkObject> waveEnemies = wave.Enemies;
            List<Transform> spawnPositions = wave.SpawnPositions;

            for (int i = 0; i < waveEnemies.Count; i++)
            {
                NetworkObject enemyPrefab = waveEnemies[i];

                // 各敵ごとのスポーン位置を設定
                Transform spawnPosition = (spawnPositions.Count > i) ? spawnPositions[i] : spawnPositions[spawnPositions.Count - 1];

                NetworkObject spawnedEnemy = runner.Spawn(enemyPrefab, spawnPosition.position, Quaternion.identity);
                _spawnedEnemies.Add(spawnedEnemy);
            }

        }

        if (_currentWave == 2)
        {
            print("Wave２になりました(ローカル)");
            RPC_WaveGeteOpen();
        }

        Debug.Log($"{_spawnedEnemies.Count} 体のエネミーをウェーブ {waveIndex + 1} にスポーンしました");
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    private void RPC_WaveGeteOpen()
    {
        print("2Waveをクリアしました。ゲート開放！(ネットワーク)");
        _wavePartitionEND.SetActive(true);
        _wavePartition.SetActive(false);
    }

    /// <summary>
    /// 全てのエネミーが非表示かを確認
    /// </summary>
    private bool AllEnemiesHidden()
    {
        foreach (NetworkObject enemy in _spawnedEnemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    // INetworkRunnerCallbacksの必要なメソッド
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
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
}
