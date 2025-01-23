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

    [SerializeField, Tooltip("�l�b�g���[�N�����i�[�v���n�u")]
    private NetworkRunner _networkRunnerPrefab = default;

    [SerializeField]
    private NetworkObject _bossEnemyOBJ = default;

    [SerializeField]
    private Transform _bossStartPos = default;


    private async void Awake()
    {
        print("��������������");
        NetworkRunner networkRunner = Instantiate(_networkRunnerPrefab);
        networkRunner.AddCallbacks(this);

        StartGameResult result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>()
        });

        // ������ComboCounter�𐶐�
        if (networkRunner.IsServer)
        {
            SpawnComboCounter(networkRunner);
        }
    }

    // ComboCounter�𐶐����郁�\�b�h(�X�|�i�[��p)
    private void SpawnComboCounter(NetworkRunner runner)
    {
        print("�{�X��������");
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
    /// �v���C���[���Q�������Ƃ��̏���
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="player"></param>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// �v���C���[���ޏo�������̏���
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
