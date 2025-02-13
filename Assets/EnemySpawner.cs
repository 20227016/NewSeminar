using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [SerializeField, Tooltip("�^�񒆂�EF���Ǘ�����I�u�W�F�N�g")]
    private GameObject _wave2ClearManager = default;

    [SerializeField, Tooltip("�G�l�~�[�̃E�F�[�u���Ƃ̐ݒ�")]
    private List<EnemyWave> _enemyWaves = new List<EnemyWave>();

    [SerializeField,Tooltip("�e���|�[�^�[�I�u�W�F�N�g")]
    private NetworkObject _bossTeleportOBJ = default;

    [SerializeField, Tooltip("�e���|�[�^�[Pos�̃I�u�W�F�N�g")]
    private Transform _bossTeleportPos = default;

    // �{�X�֘A�̃��X�g
    [SerializeField]
    private List<NetworkObject> _bossObjList = new List<NetworkObject>();

    // �{�X�֘A��pos�Ǘ����X�g
    [SerializeField]
    private List<GameObject> _bossObjPosList = new List<GameObject>();

    private bool _spawnEnd = default;

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

    private GameLauncher _gameLauncher = default;

    private NetworkRunner _networkRunner = default;

    private bool _isStart = false;

    private bool _normalClear = false;

    /// <summary>
    /// ��������
    /// </summary>
    private void Start()
    {
        _gameLauncher = FindObjectOfType<GameLauncher>();
        
        _gameLauncher.StartGameSubject.Subscribe(_ => EnemyStart());

    }

    private void BossDefeat()
    {
        NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();

        foreach (var networkObject in networkObjects)
        {
            if (networkObject != null)
            {
                _networkRunner.Despawn(networkObject);
            }
        }

        if (_networkRunner != null && _networkRunner.IsRunning)
        {
            _networkRunner.Shutdown(); // �l�b�g���[�N���V���b�g�_�E��
        }

        // �V�[���J�ڂ�S�N���C�A���g�ɒʒm
        SceneManager.LoadScene("GameClear");
    }


    private void EnemyStart()
    {

        _networkRunner = _gameLauncher.NetworkRunner;

        StartWave(_networkRunner);

        _isStart = true;

        print(_isStart + "_isStart�̏��");

        // �G�l�~�[�S�Ŏ��̒ʒm���w�ǂ��A���̃E�F�[�u��
        OnEnemiesDefeated.Subscribe(_ => NextWave(_networkRunner));

        
    }

    /// <summary>
    /// �S�ẴG�l�~�[�̑S�Ŋm�F
    /// </summary>
    private void Update()
    {

        if(_isStart)
        {
            if(_networkRunner.IsServer)
            {
                if (_spawnedEnemies.Count > 0 && AllEnemiesHidden())
                {
                    OnEnemiesDefeated.OnNext(Unit.Default);
                }
            }
        }

    }

    /// <summary>
    /// �w�肳�ꂽ�E�F�[�u�̃G�l�~�[���X�|�[��
    /// </summary>
    private void StartWave(NetworkRunner runner)
    {


        if (_enemyWaves == null || _enemyWaves.Count == 0)
        {
            Debug.LogError("�G�l�~�[�E�F�[�u���X�g���ݒ肳��Ă��܂���I");
            return;
        }
        if (_enemyWaves.Count == 0)
        {
            Debug.LogWarning("�G�l�~�[�E�F�[�u���ݒ肳��Ă��܂���I");
            return;
        }

        _currentWave = 0; // �����E�F�[�u���Z�b�g
        SpawnEnemies(runner, _currentWave);

        // �{�X�֘A�̏��������s����
        BossStage(runner);
    }

    /// <summary>
    /// ���̃E�F�[�u�֐i��
    /// </summary>
    private void NextWave(NetworkRunner runner)
    {

        print("�l�N�X�g�E�F�[�u����");

        _currentWave++;

        if (_currentWave >= _enemyWaves.Count)
        {
            Debug.Log("���ׂẴE�F�[�u���I�����܂����I");

            if(!_normalClear)
            {
                runner.Spawn(_bossTeleportOBJ, _bossTeleportPos.position, _bossTeleportOBJ.transform.rotation);
                _normalClear = true;
            }


            StartCoroutine(BossStageStartCoroutine());

            return;
        }

        Debug.Log($"�E�F�[�u {_currentWave + 1} �J�n�I");
        SpawnEnemies(runner, _currentWave);

    }

    private IEnumerator BossStageStartCoroutine()
    {
        yield return new WaitForSeconds(2f);
        OnAllEnemiesDefeated.OnNext(Unit.Default);
    }

    /// <summary>
    /// Wave���ƂɃG�l�~�[���X�|�[��������
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="waveIndex"></param>
    private void SpawnEnemies(NetworkRunner runner, int waveIndex)
    {
        if (_enemyWaves == null || _enemyWaves.Count == 0)
        {
            Debug.LogError("�G�l�~�[�E�F�[�u���X�g���ݒ肳��Ă��܂���I");
            return;
        }

        if (runner != null && runner.IsRunning)
        {
            Debug.Log("NetworkRunner �͎��s���ł��B");
        }
        else
        {
            Debug.Log("NetworkRunner �͎��s����Ă��܂���B");
        }

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



        if (_currentWave == 2)
        {
            print("�E�F�[�u�Q�B_wave2ClearManager�̐������J�n���܂�" + _wave2ClearManager);
            // WaveClear�𐶐�����
            runner.Spawn(_wave2ClearManager);
            print("�E�F�[�u�Q�B_wave2ClearManager�̐������J�n���܂����" + _wave2ClearManager);
        }

        Debug.Log($"{_spawnedEnemies.Count} �̂̃G�l�~�[���E�F�[�u {waveIndex + 1} �ɃX�|�[�����܂���");
    }

    /// <summary>
    /// �S�ẴG�l�~�[����\�������m�F
    /// </summary>
    private bool AllEnemiesHidden()
    {
        bool allNull = true;

        foreach (NetworkObject enemy in _spawnedEnemies)
        {
            if (enemy != null) // enemy�����݂���Ȃ�
            {
                allNull = false; // �S��null�ł͂Ȃ�
                if (enemy.gameObject.activeSelf) // 1�ł��\������Ă���G�l�~�[��������
                {
                    return false; // ������ false ��Ԃ�
                }
            }
        }

        return allNull; // �S��null�Ȃ� true
    }

    private void BossStage(NetworkRunner runner)
    {
        if (!_spawnEnd)
        {
            for (int i = 0; i < _bossObjList.Count; i++)
            {
                NetworkObject enemyPrefab = _bossObjList[i];
                runner.Spawn(enemyPrefab, _bossObjPosList[i].transform.position, Quaternion.identity);

            }
            _spawnEnd = true;
        }

        BossDemo bossDemo = FindObjectOfType<BossDemo>();

        if (bossDemo != null)
        {
            bossDemo.BossDeathSubject.Subscribe(_ => BossDefeat());
        }
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
