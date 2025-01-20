using UnityEngine;
using UniRx;
using System;
using Fusion;

/// <summary>
/// ステージシーンを管理するクラス
/// </summary>
public class StageManager : NetworkBehaviour
{
    [SerializeField]
    private StageEnemyManagement _enemyManagement = default; // 敵管理クラス

    [SerializeField]
    private PlayersGather _playersGather = default; // プレイヤー集合管理クラス

    [SerializeField]
    private Vector3 _teleportPosition = new Vector3(0, 0, 0); // テレポート先座標(あとで決める(ボス部屋座標))

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        if (_enemyManagement == null || _playersGather == null)
        {
            Debug.LogError("EnemyManagementやPlayersGatherを確認してください。NULLになっています");
        }
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        // 敵全滅とプレイヤー集合の両方を待つ
        Observable.CombineLatest(
            _enemyManagement.AllEnemiesDefeated,
            _playersGather.PlayerGather
        )
        .Where(_ => Object.HasStateAuthority) // サーバー側で実行
        .Subscribe(_ => HandleAllEnemiesDefeated())
        .AddTo(this);
    }

    /// <summary>
    /// 敵が全滅し、プレイヤーが集合した場合の処理
    /// </summary>
    private void HandleAllEnemiesDefeated()
    {
        Debug.Log("敵全滅＋プレイヤーの集合を確認。テレポート処理を開始します。");

        foreach (var player in Runner.ActivePlayers)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject networkObject))
            {
                networkObject.transform.position = _teleportPosition;
            }
        }

        Debug.Log("全プレイヤーのテレポートが完了しました！");
    }
}
