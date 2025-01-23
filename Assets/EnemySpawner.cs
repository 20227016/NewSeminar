using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Collections;

public class EnemySpawner : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField, Tooltip("ネットワークランナープレハブ")]
    private NetworkRunner _networkRunnerPrefab = default;

    [SerializeField]
    private NetworkObject _bossEnemyOBJ = default;

    [SerializeField]
    private Transform _bossStartPos = default;


    private async void Awake()
    {
        print("初期処理したよ");
        NetworkRunner networkRunner = Instantiate(_networkRunnerPrefab);
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
    }

    // ComboCounterを生成するメソッド(スポナー専用)
    private void SpawnComboCounter(NetworkRunner runner)
    {
        print("ボス生成した");
        runner.Spawn(_bossEnemyOBJ, _bossStartPos.position, Quaternion.identity);

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
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// プレイヤーが退出した時の処理
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="player"></param>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }
}
