using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UniRx;

public class EnemySpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField, Tooltip("ボスへ向かうテレポーター")]
    private GameObject _bossTeleporter = default;

    [SerializeField, Tooltip("ネットワークランナープレハブ")]
    private NetworkRunner _networkRunnerPrefab = default;

    [SerializeField, Tooltip("エネミーのプレハブリスト")]
    private List<NetworkObject> _enemyPrefabs = new List<NetworkObject>();

    [SerializeField, Tooltip("エネミーのスポーン位置リスト")]
    private List<Transform> _enemyStartPositions = new List<Transform>();


    
    private List<NetworkObject> _spawnedEnemies = new List<NetworkObject>();

    private Subject<Unit> OnAllEnemiesDefeated = new Subject<Unit>();
    public IObservable<Unit> OnAllEnemiesDefeatedObservable => OnAllEnemiesDefeated;

    private void Start()
    {

        GameLauncher gameLauncher = FindObjectOfType<GameLauncher>();
        NetworkRunner networkRunner = gameLauncher.NetworkRunner;
        Debug.Log(networkRunner);

        gameLauncher.StartGameSubject.Subscribe(_ => InitialSpawn(networkRunner));

        // エネミー全滅時の通知を購読
        OnAllEnemiesDefeated.Subscribe(_ => HandleAllEnemiesDefeated());
    }

    /// <summary>
    /// すべてのエネミーの状態(表示・非表示)を確認する 
    /// </summary>
    private void Update()
    {
        if (_spawnedEnemies.Count > 0 && AllEnemiesHidden())
        {
            OnAllEnemiesDefeated.OnNext(Unit.Default);
        }
    }

    /// <summary>
    /// エネミーの初期スポーン処理
    /// </summary>
    private void InitialSpawn(NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            for (int i = 0; i < _enemyPrefabs.Count; i++)
            {
                // エネミーをスポーン
                var spawnedEnemy = runner.Spawn(_enemyPrefabs[i], _enemyStartPositions[i].position, Quaternion.identity);
                _spawnedEnemies.Add(spawnedEnemy);

                // エネミーの非表示を監視
                spawnedEnemy.gameObject.SetActive(true);
            }

            Debug.Log($"{_spawnedEnemies.Count}体のエネミーをスポーンしました");
        }
    }

    /// <summary>
    /// 全てのエネミーが非表示かを確認
    /// </summary>
    /// <returns>全エネミーが非表示ならtrue</returns>
    private bool AllEnemiesHidden()
    {
        foreach (var enemy in _spawnedEnemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 全エネミーが非表示になったときの処理
    /// </summary>
    private void HandleAllEnemiesDefeated()
    {
        Debug.Log("全エネミーが倒されました！");
        if (_bossTeleporter != null)
        {
            _bossTeleporter.SetActive(true);
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
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    /// <summary>
    /// プレイヤーが参加したときの処理
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="player"></param>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

    /// <summary>
    /// プレイヤーが退出した時の処理
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="player"></param>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
}
