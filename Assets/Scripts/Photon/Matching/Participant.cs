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
    /// 参加者が増えたときにルーム管理に追加
    /// 呼び出し:ホストとクライアント
    /// 実行する:ホスト
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void RPC_ParticipantCountAdd()
    {

        Debug.Log($"呼び出したオブジェクト:{this.gameObject}");
        // データ初期化
        _roomInfo.RoomName = _roomInfo.RoomName;
        _roomInfo.MaxParticipantCount = _roomInfo.MaxParticipantCount;
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount;
        // 更新
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount + 1;
        Debug.Log($"ルーム参加人数変更:{_roomInfo.CurrentParticipantCount}");

    }

    /// <summary>
    /// 参加者が減った時にルーム管理から削除
    /// 呼び出し:ホストとクライアント
    /// 実行する:ホスト
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void RPC_ParticipantCountRemove()
    {

        Debug.Log($"呼び出したオブジェクト:{this.gameObject}");
        // データ初期化
        _roomInfo.RoomName = _roomInfo.RoomName;
        _roomInfo.MaxParticipantCount = _roomInfo.MaxParticipantCount;
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount;
        // 更新
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount + 1;
        Debug.Log($"ルーム参加人数変更:{_roomInfo.CurrentParticipantCount}");

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
