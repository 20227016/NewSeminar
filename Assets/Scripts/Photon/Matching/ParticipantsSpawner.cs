using Fusion;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// ルームランナースポナー（引数に生成するオブジェクトにクライアントとして生成かホストとして生成かを決めれる）
/// 生成した後１つのスクリプトで処理をするので、If分でクライアントの時の処理とホストの時の処理を分ける
/// ホスト
/// ネットワーク管理全部（ステートオーソリティーを持っている（もてるのはホストのみ））
/// 人数管理、RPC処理、ゲームスタート
/// 
/// クライアント
/// 変更時に通知を送る
/// 
/// ルームマネージャーがなくなりルームマネージャーのRPC処理をホストに
/// 
/// 
/// </summary>
public class ParticipantsSpawner : MonoBehaviour, IParticipantsSpawner
{

    [SerializeField, Tooltip("Participantsのプレハブ")]
    private GameObject _participantPrefab = default;

    /// <summary>
    /// ホストへ生成した通知
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Spawner(NetworkRunner networkRunner, RoomInfo preDefinedRoom)
    {

        print("スポナーへようこそ");

        // セッション方法
        StartGameArgs startGameArgs = default;
        // セッション実行結果
        StartGameResult result = default;
        // サーバー情報（Args＝引数）
        startGameArgs = new StartGameArgs()
        {

            // ゲームモード
            GameMode = GameMode.Host,
            // セッション名
            SessionName = "Room",
            // ネットワーク上でのシーン遷移同期？
            SceneManager = this.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            // セッション作成時に、現在のシーンに置かれたシーンオブジェクトをスポーンする
            Scene = SceneManager.GetActiveScene().buildIndex

        };
        Debug.Log($"ホスト　で{startGameArgs}　の設定で参加処理実行");
        // 指定したネットワークモードでのゲームをスタートさせる（セッション名が重複している等で失敗する）
        result = await networkRunner.StartGame(startGameArgs);
        Debug.Log($"結果：{result}");
        // セッションが成功したか
        if (result.Ok)
        {

            Debug.Log($"ホスト　として部屋 {startGameArgs.SessionName} に正常に参加しました。");

        }
        else
        {

            Debug.LogError($"ホスト　として部屋 {startGameArgs.SessionName} に参加できませんでした: {result.ShutdownReason}");
            Debug.LogError($"セッションを解除し終了します");
            await networkRunner.Shutdown();
            // サーバー情報（Args＝引数）
            startGameArgs = new StartGameArgs()
            {

                // ゲームモード
                GameMode = GameMode.Client,
                // セッション名
                SessionName = "Room",
                // ネットワーク上でのシーン遷移同期？
                SceneManager = this.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                // セッション作成時に、現在のシーンに置かれたシーンオブジェクトをスポーンする
                Scene = SceneManager.GetActiveScene().buildIndex

            };
            Debug.Log($"クライアント　で{startGameArgs}　の設定で参加処理実行");

            // 指定したネットワークモードでのゲームをスタートさせる（セッション名が重複している等で失敗する）
            result = await networkRunner.StartGame(startGameArgs);
            Debug.Log($"結果：{result}");
            if (result.Ok)
            {

                Debug.Log($"クライアント　として部屋 {startGameArgs.SessionName} に正常に参加しました。");

            }
            else
            {

                Debug.LogError($"クライアントとして　部屋 {startGameArgs.SessionName} に参加できませんでした: {result.ShutdownReason}");
                Debug.LogError($"接続を解除し終了します");
                await networkRunner.Shutdown();
                return false;

            }

        }
        if (startGameArgs.GameMode == GameMode.Client)
        {

            Debug.Log($"入室検査");
            Debug.Log($"セッション状態：{networkRunner.SessionInfo}");
            Debug.Log($"現在の参加数: {preDefinedRoom.CurrentParticipantCount}");
            Debug.Log($"最大参加人数: {preDefinedRoom.MaxParticipantCount}");
            if (preDefinedRoom.CurrentParticipantCount == preDefinedRoom.MaxParticipantCount + 1)
            {

                Debug.LogError($"人数が最大に達しているため部屋 {startGameArgs.SessionName} に参加できませんでした: {result.ShutdownReason}");
                await networkRunner.Shutdown();
                return false;

            }
            Debug.Log($"人数に余裕があるため入室できます");

        }

        Debug.Log($"ホスト・クライアント各処理");
        // 参加者オブジェクト
        NetworkObject participant = default;
        // 参加者の参加形態による処理
        switch (networkRunner.GameMode)
        {

            //　ホスト
            case GameMode.Host:

                // 参加者をホストとして生成
                participant = networkRunner.Spawn(_participantPrefab, Vector3.zero, Quaternion.identity, networkRunner.LocalPlayer);
                // 生成に失敗したとき
                if (participant == null)
                {

                    Debug.LogError("参加者オブジェクトの生成に失敗");
                    return false;

                }
                Debug.Log("ホストとして部屋に参加しました。");
                if (participant.HasStateAuthority)
                {

                    Debug.Log("ホストの権限取得に成功");

                }
                else
                {

                    Debug.LogError("ホストの権限取得に失敗");
                    return false;

                }
                // Room情報をわたすインターフェース
                IParticipantInfo iParticipantInfo = participant.GetComponent<IParticipantInfo>();
                if (iParticipantInfo == null)
                {

                    Debug.LogError("ホストにRoom情報を渡すためのインターフェースがない");
                    return false;

                }
                iParticipantInfo.SetRoomInfo(preDefinedRoom);

                break;
            // クライアント
            case GameMode.Client:

                // 参加者をクライアントとして生成
                participant = networkRunner.Spawn(_participantPrefab);
                // 生成に失敗したとき
                if (participant == null)
                {

                    Debug.LogError("参加者オブジェクトの生成に失敗");
                    return false;

                }
                Debug.Log("クライアントとして部屋に参加しました。");

                break;

        }
        return true;



    }
}
