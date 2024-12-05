using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
    public async Task<bool> Spawner(StartGameArgs startGameArgs, NetworkRunner _networkRunner, RoomInfo preDefinedRoom)
    {

        print("スポナーへようこそ");
        try
        {

            // 指定したネットワークモードでのゲームをスタートさせる（セッション名が重複している等で失敗する）
            StartGameResult result = await _networkRunner.StartGame(startGameArgs);
            // セッションが成功したか
            if (result.Ok)
            {

                Debug.Log($"部屋 {startGameArgs.SessionName} に正常に参加しました。");

            }
            else
            {

                Debug.LogError($"部屋 {startGameArgs.SessionName} に参加できませんでした: {result.ShutdownReason}");
                Debug.LogError($"接続を解除し終了します");
                await _networkRunner.Shutdown();
                return false;

            }
            // 参加者オブジェクト
            NetworkObject participant = default;
            // 参加者の参加形態による処理
            switch (_networkRunner.GameMode)
            {

                //　ホスト
                case GameMode.Host:

                    // 参加者をホストとして生成
                    participant = _networkRunner.Spawn(_participantPrefab, Vector3.zero, Quaternion.identity, _networkRunner.LocalPlayer);
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
                    participant = _networkRunner.Spawn(_participantPrefab);
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
        catch
        {

            Debug.LogError("エラー");
            // 生成失敗
            return false;


        }

    }

}
