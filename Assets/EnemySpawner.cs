using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UniRx;

public class EnemySpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField, Tooltip("�{�X�֌������e���|�[�^�[")]
    private GameObject _bossTeleporter = default;

    [SerializeField, Tooltip("�l�b�g���[�N�����i�[�v���n�u")]
    private NetworkRunner _networkRunnerPrefab = default;

    [SerializeField, Tooltip("�G�l�~�[�̃v���n�u���X�g")]
    private List<NetworkObject> _enemyPrefabs = new List<NetworkObject>();

    [SerializeField, Tooltip("�G�l�~�[�̃X�|�[���ʒu���X�g")]
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

        // �G�l�~�[�S�Ŏ��̒ʒm���w��
        OnAllEnemiesDefeated.Subscribe(_ => HandleAllEnemiesDefeated());
    }

    /// <summary>
    /// ���ׂẴG�l�~�[�̏��(�\���E��\��)���m�F���� 
    /// </summary>
    private void Update()
    {
        if (_spawnedEnemies.Count > 0 && AllEnemiesHidden())
        {
            OnAllEnemiesDefeated.OnNext(Unit.Default);
        }
    }

    /// <summary>
    /// �G�l�~�[�̏����X�|�[������
    /// </summary>
    private void InitialSpawn(NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            for (int i = 0; i < _enemyPrefabs.Count; i++)
            {
                // �G�l�~�[���X�|�[��
                var spawnedEnemy = runner.Spawn(_enemyPrefabs[i], _enemyStartPositions[i].position, Quaternion.identity);
                _spawnedEnemies.Add(spawnedEnemy);

                // �G�l�~�[�̔�\�����Ď�
                spawnedEnemy.gameObject.SetActive(true);
            }

            Debug.Log($"{_spawnedEnemies.Count}�̂̃G�l�~�[���X�|�[�����܂���");
        }
    }

    /// <summary>
    /// �S�ẴG�l�~�[����\�������m�F
    /// </summary>
    /// <returns>�S�G�l�~�[����\���Ȃ�true</returns>
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
    /// �S�G�l�~�[����\���ɂȂ����Ƃ��̏���
    /// </summary>
    private void HandleAllEnemiesDefeated()
    {
        Debug.Log("�S�G�l�~�[���|����܂����I");
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
