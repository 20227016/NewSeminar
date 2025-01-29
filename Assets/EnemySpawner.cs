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
    [Tooltip("���̃E�F�[�u�ŃX�|�[������G�l�~�[�̃��X�g")]
    public List<NetworkObject> Enemies = new List<NetworkObject>();

    [Tooltip("���̃E�F�[�u�̃X�|�[���ʒu���X�g")]
    public List<Transform> SpawnPositions = new List<Transform>();
}

/// <summary>
/// �G�l�~�[�̃X�|�[�����Ǘ�����X�N���v�g
/// </summary>
public class EnemySpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField, Tooltip("�{�X�֌������e���|�[�^�[")]
    private GameObject _bossTeleporter = default;

    [SerializeField, Tooltip("�X�e�[�W���킯��d�؂�")]
    private GameObject _wavePartition = default;

    [SerializeField, Tooltip("�X�e�[�W���킯��d�؂�")]
    private GameObject _wavePartitionEND = default;

    [SerializeField, Tooltip("�l�b�g���[�N�����i�[�v���n�u")]
    private NetworkRunner _networkRunnerPrefab = default;

    [SerializeField, Tooltip("�G�l�~�[�̃E�F�[�u���Ƃ̐ݒ�")]
    private List<EnemyWave> _enemyWaves = new List<EnemyWave>();

    // �X�|�[���ς݃G�l�~�[���Ǘ�
    private List<NetworkObject> _spawnedEnemies = new List<NetworkObject>(); 

    // Wave�N���A�ʒm
    private Subject<Unit> OnEnemiesDefeated = new Subject<Unit>();
    public IObservable<Unit> OnEnemiesDefeatedObservable => OnEnemiesDefeated;

    // �m�[�}���X�e�[�W�N���A�ʒm
    private Subject<Unit> OnAllEnemiesDefeated = new Subject<Unit>();
    public IObservable<Unit> OnAllEnemiesDefeatedObservable => OnAllEnemiesDefeated;

    // ���݂̃E�F�[�u�ԍ�
    [Networked]
    private int _currentWave { get; set; } = 0;

    /// <summary>
    /// ��������
    /// </summary>
    private void Start()
    {
        GameLauncher gameLauncher = FindObjectOfType<GameLauncher>();
        NetworkRunner networkRunner = gameLauncher.NetworkRunner;

        gameLauncher.StartGameSubject.Subscribe(_ => StartWave(networkRunner));

        // �G�l�~�[�S�Ŏ��̒ʒm���w�ǂ��A���̃E�F�[�u��
        OnEnemiesDefeated.Subscribe(_ => NextWave(networkRunner));
    }

    /// <summary>
    /// �S�ẴG�l�~�[�̑S�Ŋm�F
    /// </summary>
    private void Update()
    {
        if (_spawnedEnemies.Count > 0 && AllEnemiesHidden())
        {
            OnEnemiesDefeated.OnNext(Unit.Default);
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�E�F�[�u�̃G�l�~�[���X�|�[��
    /// </summary>
    private void StartWave(NetworkRunner runner)
    {
        if (_enemyWaves.Count == 0)
        {
            Debug.LogWarning("�G�l�~�[�E�F�[�u���ݒ肳��Ă��܂���I");
            return;
        }

        _currentWave = 0; // �����E�F�[�u���Z�b�g
        SpawnEnemies(runner, _currentWave);
    }

    /// <summary>
    /// ���̃E�F�[�u�֐i��
    /// </summary>
    private void NextWave(NetworkRunner runner)
    {
        _currentWave++;



        if (_currentWave >= _enemyWaves.Count)
        {
            Debug.Log("���ׂẴE�F�[�u���I�����܂����I");
            if (_bossTeleporter != null)
            {
                OnAllEnemiesDefeated.OnNext(Unit.Default);
                // �{�X�e���|�[�^�[��L����
                _bossTeleporter.SetActive(true); 
            }
            return;
        }

        Debug.Log($"�E�F�[�u {_currentWave + 1} �J�n�I");
        SpawnEnemies(runner, _currentWave);
    }

    /// <summary>
    /// Wave���ƂɃG�l�~�[���X�|�[��������
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="waveIndex"></param>
    private void SpawnEnemies(NetworkRunner runner, int waveIndex)
    {
        if (runner.IsServer)
        {
            _spawnedEnemies.Clear();

            // ���̃E�F�[�u�f�[�^
            EnemyWave wave = _enemyWaves[waveIndex];
            List<NetworkObject> waveEnemies = wave.Enemies;
            List<Transform> spawnPositions = wave.SpawnPositions;

            for (int i = 0; i < waveEnemies.Count; i++)
            {
                NetworkObject enemyPrefab = waveEnemies[i];

                // �e�G���Ƃ̃X�|�[���ʒu��ݒ�
                Transform spawnPosition = (spawnPositions.Count > i) ? spawnPositions[i] : spawnPositions[spawnPositions.Count - 1];

                NetworkObject spawnedEnemy = runner.Spawn(enemyPrefab, spawnPosition.position, Quaternion.identity);
                _spawnedEnemies.Add(spawnedEnemy);
            }

        }

        if (_currentWave == 2)
        {
            print("Wave�Q�ɂȂ�܂���(���[�J��)");
            RPC_WaveGeteOpen();
        }

        Debug.Log($"{_spawnedEnemies.Count} �̂̃G�l�~�[���E�F�[�u {waveIndex + 1} �ɃX�|�[�����܂���");
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    private void RPC_WaveGeteOpen()
    {
        print("2Wave���N���A���܂����B�Q�[�g�J���I(�l�b�g���[�N)");
        _wavePartitionEND.SetActive(true);
        _wavePartition.SetActive(false);
    }

    /// <summary>
    /// �S�ẴG�l�~�[����\�������m�F
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

    // INetworkRunnerCallbacks�̕K�v�ȃ��\�b�h
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
