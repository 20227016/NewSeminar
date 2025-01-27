using System.Collections.Generic;
using UnityEngine;

public class BossStageTransfer : MonoBehaviour
{
    // テレポート内のプレイヤーを管理するリスト
    private List<GameObject> _playersInPortal = new List<GameObject>();

    [SerializeField]
    private StageEnemyManagement _enemyManagement = default; // 敵管理クラス

    // 必要なプレイヤー数
    [SerializeField, Tooltip("ノーマルステージにテレポートするために必要な人数")]
    private int _bossStageRequiredPlayers = 1; // 必要なプレイヤー数

    [SerializeField, Tooltip("ボスステージのテレポート座標")]
    private Transform _bossTeleportPos = default;


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
        if (_playersInPortal.Count >= _bossStageRequiredPlayers)
        {
            BossTeleportAllPlayers();
        }
    }

    private void BossTeleportAllPlayers()
    {
        // ボスステージにテレポート
        foreach (GameObject player in _playersInPortal)
        {

            player.transform.position = _bossTeleportPos.position;
            print($"{player.name} をボス部屋にテレポートしました");
        }
    }
}
