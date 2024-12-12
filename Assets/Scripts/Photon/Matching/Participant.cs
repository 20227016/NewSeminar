using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class Participant : NetworkBehaviour, IRoomController
{

    /// <summary>
    /// 部屋情報
    /// </summary>
    private RoomInfo _roomInfo = default;

    private NetworkObject _networkObject = default;

    private void Start()
    {

        _networkObject = this.GetComponent<NetworkObject>();
        if(_networkObject == null)
        {

            Debug.LogError($"自分についているネットワークオブジェクトが見つかりません");

        }
        // 自分がホストオブジェクトの時
        if (_networkObject.HasStateAuthority)
        {

            Debug.Log($"ホストだよ");
            _roomInfo = GameObject.Find("Room").GetComponent<RoomInfo>();
            if (_roomInfo == null)
            {

                Debug.LogError($"ルーム管理クラスが見つかりません");

            }
            // データ初期化
            _roomInfo.RoomName = _roomInfo.RoomName;
            _roomInfo.MaxParticipantCount = _roomInfo.MaxParticipantCount;
            _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount;
            // 名前の変更
            this.gameObject.name = $"{_roomInfo.CurrentParticipantCount}_Host";

        }
        else
        {

            // 名前の変更
            this.gameObject.name = $"{_roomInfo.CurrentParticipantCount}_Client";

        }
       
    }

    /// <summary>
    /// 参加者が書かれるテキストの名前を共有
    /// </summary>
    private void NameShare()
    {



    }

    /// <summary>
    /// 参加者が増えたときにルーム管理に追加
    /// 呼び出し:ホストとクライアント
    /// 実行する:ホスト
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public async void RPC_ParticipantCountAdd()
    {

        Debug.Log("クライアントでも実行");
        await GetRoomAwait();
        Debug.Log($"呼び出したオブジェクト:{this.gameObject}");
        // 更新
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount + 1;
        Debug.Log($"ルーム参加人数変更:{_roomInfo.CurrentParticipantCount}");
        if(_roomInfo.CurrentParticipantCount == 1)
        {

            GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<Text>().text = $"Host_{_roomInfo.CurrentParticipantCount}";

        }
        else
        {

            GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<Text>().text = $"Client_{_roomInfo.CurrentParticipantCount}";

        }
        

    }

    /// <summary>
    /// 参加者が減った時にルーム管理から削除
    /// 呼び出し:ホストとクライアント
    /// 実行する:ホスト
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public async void RPC_ParticipantCountRemove()
    {

        await GetRoomAwait();
        Debug.Log($"呼び出したオブジェクト:{this.gameObject}");
        if(_roomInfo.CurrentParticipantCount == 1)
        {

            GameObject.Find($"Host_{_roomInfo.CurrentParticipantCount}").GetComponent<Text>().text = $"Participant_{_roomInfo.CurrentParticipantCount}";

        }
        else
        {

            GameObject.Find($"Client_{_roomInfo.CurrentParticipantCount}").GetComponent<Text>().text = $"Participant_{_roomInfo.CurrentParticipantCount}";

        }
        // 更新
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount - 1;
        Debug.Log($"ルーム参加人数変更:{_roomInfo.CurrentParticipantCount}");

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async Task GetRoomAwait()
    {

        while(_roomInfo == null)
        {

            await Task.Delay(100); // 100msごとにチェック

        }
        

    }

}
