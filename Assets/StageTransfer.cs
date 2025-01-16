using Fusion;
using UnityEngine;

public class StageTransfer : NetworkBehaviour
{
    private int _playersInPortal = 0;       // コライダー内のプレイヤー数
    private const int _requiredPlayers = 4; // 必要なプレイヤー数

    private void Start()
    {
        if (Runner == null)
        {
            Debug.LogError("シーン読み込みを検知、スタート時のみの現状報告");
            Debug.LogError("Runnerが設定されていません。このスクリプトは正しく動作しない可能性があります。");
        }
    }


    /// <summary>
    /// 転送ポータルにプレイヤーが入ったときに人数を加算する
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            // コライダーに入ったプレイヤーをカウント
            if (Object.HasStateAuthority)
            {
                _playersInPortal++;
                CheckTeleportCondition();
                print("プレイヤーを検知。現在の人数は" + _playersInPortal + "です");
            }
        }
    }

    /// <summary>
    /// 転送ポータルからプレイヤーが抜けたときに人数を減少させる
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            // コライダーから出たプレイヤーをカウント
            if (Object.HasStateAuthority)
            {
                _playersInPortal--;
                print("現在の人数.変更を検知。現在の人数は" + _playersInPortal + "です");
            }
        }
    }

    /// <summary>
    /// 必要人数が転送ポータルに入ったとき、全員を指定ポイントまでテレポートさせる
    /// </summary>
    private void CheckTeleportCondition()
    {
        // 必要人数が揃ったらテレポートを実行する
        if (_playersInPortal >= _requiredPlayers)
        {
            TeleportAllPlayers();
        }
    }

    /// <summary>
    /// テレポート処理
    /// </summary>
    private void TeleportAllPlayers()
    {

        print("指定人数を検知。テレポートを開始します");

        // 全プレイヤーを指定ポイントまで移動させる
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject networkObject))
            {

                // テレポート先の位置を設定
                Vector3 teleportPosition = new Vector3(0, 0, 0); // 詳しい座標はあとで設定する
                networkObject.transform.position = teleportPosition;
            }
            print("すべてのプレイヤーのテレポートに成功しました");
        }
    }
}
