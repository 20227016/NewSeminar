using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class NormalStageTransfer : NetworkBehaviour
{
    private NetworkRunner _runner = default;

    // テレポート内のプレイヤーを管理するリスト
    private List<GameObject> _playersInPortal = new List<GameObject>();

    private StageEnemyManagement _enemyManagement = default; // 敵管理クラス

    [Networked]
    private bool _clearNormalStage { get; set; } = false;

    // 必要なプレイヤー数
    [Networked,Tooltip("ノーマルステージにテレポートするために必要な人数")]
    private int _normalStageRequiredPlayers { get; set; }= 4; // 必要なプレイヤー数

    private GameObject _normalTeleportPosition = default;
    private GameObject _bossTeleportPosition = default;

    [Tooltip("ノーマルステージのテレポート座標")]
    private Transform _normalStageteleportPos = default;

    [Tooltip("ボスステージのテレポート座標")]
    private Transform _bossStageteleportPos = default;

    public override void Spawned()
    {

        _enemyManagement = FindObjectOfType<StageEnemyManagement>();
        _normalTeleportPosition = GameObject.Find("NormalTeleportPosition");
        _bossTeleportPosition = GameObject.Find("BossTeleportPosition");

        _normalStageteleportPos = _normalTeleportPosition.transform;
        _bossStageteleportPos = _bossTeleportPosition.transform;

        // 敵全滅を待つ
        Observable.CombineLatest(
            _enemyManagement.AllEnemiesDefeated
        )
        .Subscribe(_ => HandleAllEnemiesDefeated())
        .AddTo(this);
    }

    /// <summary>
    /// 転送ポータルにプレイヤーが入ったときにリストに追加する
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !_playersInPortal.Contains(collider.gameObject))
        {
            _playersInPortal.Add(collider.gameObject);
            print($"プレイヤーを検知。現在の人数は {_playersInPortal.Count} です");
        }

        // 必要人数が揃ったら全員をテレポート
        if ((_playersInPortal.Count >= _normalStageRequiredPlayers) && (!_clearNormalStage))
        {
            NormalTeleportAllPlayers();
        }
        else if ((_playersInPortal.Count >= _normalStageRequiredPlayers) && (_clearNormalStage))
        {
            BossTeleportAllPlayers();
        }
    }

    /// <summary>
    /// 転送ポータルからプレイヤーが抜けたときにリストから削除する
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player") && _playersInPortal.Contains(collider.gameObject))
        {
            _playersInPortal.Remove(collider.gameObject);
            print($"プレイヤーがポータルを離れました。現在の人数は {_playersInPortal.Count} です");
        }
    }

    /// <summary>
    /// 全てのプレイヤーをテレポートさせる
    /// </summary>
    private void NormalTeleportAllPlayers()
    {
        // ノーマルステージにテレポート
        foreach (GameObject player in _playersInPortal)
        {

            player.transform.position = _normalStageteleportPos.position;
            print($"{player.name} をノーマルステージにテレポートしました");
        }
        // 一度ノーマルステージにテレポートしたらノーマルステージに行くためのテレポート人数を1人にする(再接続用)
        _normalStageRequiredPlayers = 1;
    }

    private void BossTeleportAllPlayers()
    {
        // ボスステージにテレポート
        foreach (GameObject player in _playersInPortal)
        {
            if (_normalStageRequiredPlayers != 1)
            {
                player.transform.position = _bossStageteleportPos.position;
                print($"{player.name} をボスステージに途中参加としてテレポートしました");
            }
        }
    }


    private void HandleAllEnemiesDefeated()
    {
        _clearNormalStage = true;
        print("敵全滅の通知を受け取りました" + _clearNormalStage);
    }
}
