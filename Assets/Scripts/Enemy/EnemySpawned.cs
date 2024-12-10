using UnityEngine;
using Fusion;
using UniRx;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField, Tooltip("生成するエネミーのPrefab")]
    private NetworkPrefabRef enemyPrefab;


    [SerializeField, Tooltip("生成するエネミーの数")]
    private int enemyCount = default;

    [SerializeField, Tooltip("生成エリアの中心")]
    private Transform spawnAreaCenter;

    [SerializeField, Tooltip("生成エリアの範囲")]
    private float spawnRadius = 10f;

    private NetworkRunner _runner;

    // ゲーム開始時にエネミーを生成する
    private void Start()
    {
        print("スタートは実行されています");
        GameInitializer.Instance.OnEnemySpawnRequested
            .Subscribe(_ =>
            {
                Debug.Log("EnemySpawner: エネミースポーン開始");
                SpawnerStart();
                GameInitializer.Instance.NetworkEnemySpawn();
            })
            .AddTo(this); // オブジェクト破棄時に自動解除

    }

    private async void SpawnerStart()
    {

        // NetworkRunnerを動的に生成
        _runner = gameObject.AddComponent<NetworkRunner>();
        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host, // サーバーとして起動
            SessionName = "TestSession",
        });

        if (!result.Ok)
        {
            Debug.LogError($"NetworkRunnerの起動に失敗しました: {result.ShutdownReason}");
            return;
        }
        if (_runner.IsServer) // サーバーのみエネミーを生成
        {
            print("サーバーでメソッドを実行。エネミーをスポーンします。");
            SpawnEnemies();
        }
    }

    /// <summary>
    /// エネミーを生成する処理
    /// </summary>
    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            // ランダムな位置を計算
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // エネミーを生成
            Runner.Spawn(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// 生成位置をランダムに取得
    /// </summary>
    /// <returns>ランダムな座標</returns>
    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            spawnAreaCenter.position.x + randomCircle.x,
            spawnAreaCenter.position.y,
            spawnAreaCenter.position.z + randomCircle.y
        );
    }
}
