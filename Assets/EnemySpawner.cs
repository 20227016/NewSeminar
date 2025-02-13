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

    [SerializeField, Tooltip("真ん中のEFを管理するオブジェクト")]
    private GameObject _wave2ClearManager = default;

    [SerializeField, Tooltip("エネミーのウェーブごとの設定")]
    private List<EnemyWave> _enemyWaves = new List<EnemyWave>();

    [SerializeField,Tooltip("テレポーターオブジェクト")]
    private NetworkObject _bossTeleportOBJ = default;

    [SerializeField, Tooltip("テレポーターPosのオブジェクト")]
    private Transform _bossTeleportPos = default;

    // ボス関連のリスト
    [SerializeField]
    private List<NetworkObject> _bossObjList = new List<NetworkObject>();

    // ボス関連のpos管理リスト
    [SerializeField]
    private List<GameObject> _bossObjPosList = new List<GameObject>();

    private bool _spawnEnd = default;

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

    private GameLauncher _gameLauncher = default;

    private NetworkRunner _networkRunner = default;

    private bool _isStart = false;

    private bool _normalClear = false;

    /// <summary>
    /// 初期処理
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
            _networkRunner.Shutdown(); // ネットワークをシャットダウン
        }

        // シーン遷移を全クライアントに通知
        SceneManager.LoadScene("GameClear");
    }


    private void EnemyStart()
    {

        _networkRunner = _gameLauncher.NetworkRunner;

        StartWave(_networkRunner);

        _isStart = true;

        print(_isStart + "_isStartの状態");

        // エネミー全滅時の通知を購読し、次のウェーブへ
        OnEnemiesDefeated.Subscribe(_ => NextWave(_networkRunner));

        
    }

    /// <summary>
    /// 全てのエネミーの全滅確認
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
    /// 指定されたウェーブのエネミーをスポーン
    /// </summary>
    private void StartWave(NetworkRunner runner)
    {


        if (_enemyWaves == null || _enemyWaves.Count == 0)
        {
            Debug.LogError("エネミーウェーブリストが設定されていません！");
            return;
        }
        if (_enemyWaves.Count == 0)
        {
            Debug.LogWarning("エネミーウェーブが設定されていません！");
            return;
        }

        _currentWave = 0; // 初期ウェーブをセット
        SpawnEnemies(runner, _currentWave);

        // ボス関連の処理を実行する
        BossStage(runner);
    }

    /// <summary>
    /// 次のウェーブへ進む
    /// </summary>
    private void NextWave(NetworkRunner runner)
    {

        print("ネクストウェーブだよ");

        _currentWave++;

        if (_currentWave >= _enemyWaves.Count)
        {
            Debug.Log("すべてのウェーブが終了しました！");

            if(!_normalClear)
            {
                runner.Spawn(_bossTeleportOBJ, _bossTeleportPos.position, _bossTeleportOBJ.transform.rotation);
                _normalClear = true;
            }


            StartCoroutine(BossStageStartCoroutine());

            return;
        }

        Debug.Log($"ウェーブ {_currentWave + 1} 開始！");
        SpawnEnemies(runner, _currentWave);

    }

    private IEnumerator BossStageStartCoroutine()
    {
        yield return new WaitForSeconds(2f);
        OnAllEnemiesDefeated.OnNext(Unit.Default);
    }

    /// <summary>
    /// Waveごとにエネミーをスポーンさせる
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="waveIndex"></param>
    private void SpawnEnemies(NetworkRunner runner, int waveIndex)
    {
        if (_enemyWaves == null || _enemyWaves.Count == 0)
        {
            Debug.LogError("エネミーウェーブリストが設定されていません！");
            return;
        }

        if (runner != null && runner.IsRunning)
        {
            Debug.Log("NetworkRunner は実行中です。");
        }
        else
        {
            Debug.Log("NetworkRunner は実行されていません。");
        }

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



        if (_currentWave == 2)
        {
            print("ウェーブ２。_wave2ClearManagerの生成を開始します" + _wave2ClearManager);
            // WaveClearを生成する
            runner.Spawn(_wave2ClearManager);
            print("ウェーブ２。_wave2ClearManagerの生成を開始します後ろ" + _wave2ClearManager);
        }

        Debug.Log($"{_spawnedEnemies.Count} 体のエネミーをウェーブ {waveIndex + 1} にスポーンしました");
    }

    /// <summary>
    /// 全てのエネミーが非表示かを確認
    /// </summary>
    private bool AllEnemiesHidden()
    {
        bool allNull = true;

        foreach (NetworkObject enemy in _spawnedEnemies)
        {
            if (enemy != null) // enemyが存在するなら
            {
                allNull = false; // 全てnullではない
                if (enemy.gameObject.activeSelf) // 1つでも表示されているエネミーがいたら
                {
                    return false; // すぐに false を返す
                }
            }
        }

        return allNull; // 全てnullなら true
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
