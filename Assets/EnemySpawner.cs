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


    [SerializeField, Tooltip("�G�l�~�[�̃v���n�u���X�g")]
    private List<NetworkObject> _enemyPrefabs = new List<NetworkObject>();

    [SerializeField, Tooltip("�G�l�~�[�̃X�|�[���ʒu���X�g")]
    private List<Transform> _enemyStartPositions = new List<Transform>();


    private void Start()
    {

        GameLauncher gameLauncher = FindObjectOfType<GameLauncher>();
        NetworkRunner networkRunner = gameLauncher.NetworkRunner;
        Debug.Log(networkRunner);

        gameLauncher.StartGameSubject.Subscribe(_ => InitialSpawn(networkRunner));
    }

    // ComboCounter�𐶐����郁�\�b�h(�X�|�i�[��p)
    private void InitialSpawn(NetworkRunner runner)
    {
        if (runner.IsServer)
        {

            for (int i = 0; i < _enemyPrefabs.Count; i++)
            {
                
                // �{�X���X�|�[��
                runner.Spawn(_enemyPrefabs[i], _enemyStartPositions[i].position, Quaternion.identity);
                print($"�G�l�~�[ {i} ���X�|�[�����܂���");
            }
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
    /// �v���C���[���Q�������Ƃ��̏���
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="player"></param>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

    /// <summary>
    /// �v���C���[���ޏo�������̏���
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="player"></param>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
}
