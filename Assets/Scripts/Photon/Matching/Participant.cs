using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class Participant : NetworkBehaviour,IParticipantInfo
{

    /// <summary>
    /// 部屋情報
    /// </summary>
    private RoomInfo _roomInfo = default;

    /// <summary>
    /// 自分のネットオブジェクト
    /// </summary>
    private NetworkObject _netObj = default;

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {

        // 参加人数を加算
        RPC_ParticipantCountAdd();
        _netObj = this.GetComponent<NetworkObject>();

    }

    
    /// <summary>
    /// 参加者人数を加算
    /// 呼び出し:ホストとクライアント
    /// 実行する:ホスト
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void RPC_ParticipantCountAdd()
    {

        await WaitForNotNull();
        _roomInfo.CurrentParticipantCount++;

    }

    /// <summary>
    ///  中身が入るまで待機
    /// </summary>
    /// <returns></returns>
    private async Task WaitForNotNull()
    {

        while (_roomInfo == null)
        {

            // _roomInfo が null の場合は 100ミリ秒待機して再確認
            await Task.Delay(100);

        }

    }

    /// <summary>
    /// ホストの場合にルーム情報を取得
    /// </summary>
    /// <param name="roomInfo"></param>
    public void SetRoomInfo(RoomInfo roomInfo)
    {

        this._roomInfo = roomInfo;

    }

}
