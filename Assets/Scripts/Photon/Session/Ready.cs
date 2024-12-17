using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class Ready : BaseRoom, IReady
{

    private TextMemory _textMemory = default;

    public override void Spawned()
    {

        base.Spawned();
        _textMemory = GameObject.Find("Ready").transform.Find("Text").GetComponent<TextMemory>();

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {

        if (_networkRunner.IsServer)
        {

            ChangeText();

        }

    }

    /// <summary>
    /// 呼び出したサーバーがホストかクライアントかを判断
    /// </summary>
    /// <param name="participant"></param>
    public async void ParticipantReady(NetworkObject participant)
    {

        Debug.Log($"{_roomInfo}準備");
        await GetRoomAwait();
        Debug.Log($"ルーム確保");
        Debug.Log($"{_networkRunner}ランナー");
        Debug.Log($"{_networkRunner.IsServer}サーバー");
        bool isHost = default;
        if (_networkRunner.IsServer)
        {

            Debug.Log("ホスト");
            isHost = true;

        }
        else
        {

            Debug.Log("クライアント");
            isHost = false;

        }
        RPC_ParticipantReady(isHost, participant);

    }

    /// <summary>
    /// ホストが準備完了・取り消しを担当
    /// </summary>
    /// <param name="isHost"></param>
    /// <param name="participant"></param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_ParticipantReady(bool isHost, NetworkObject participant)
    {

        if (isHost)
        {

            Debug.Log("ホスト");
            _roomInfo.ChangeReady(participant.name);
            ChangeText();
            //全員が準備を完了していないとき　
            if (!_roomInfo.GoCheck())
            {

                _roomInfo.ChangeReady(participant.name);
                ChangeText();

            }

        }
        else
        {

            Debug.Log("クライアント");

            _roomInfo.ChangeReady(participant.name);
            ChangeText();

        }

    }

    /// <summary>
    /// 準備完了人数を知らせるテキストを変える
    /// </summary>
    private void ChangeText()
    {

        // 準備完了人数
        int num = _roomInfo.ReadyGoCount();
        _textMemory.Character = $"準備完了({num}/4)";
        _textMemory.RPC_TextUpdate();

    }


}
