using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class Participation : BaseRoom, IRoomController
{

    /// <summary>
    /// 参加者が増えたときにルーム管理に追加
    /// 呼び出し:ホストとクライアント
    /// 実行する:ホスト
    /// </summary>
    public async void ParticipantAdd(PlayerRef playerRef)
    {

        // Roomクラス取得まで待つ
        await GetRoomAwait();
        Debug.Log($"参加者加算処理＿開始: {this.GetType().Name}クラス");
        Debug.Log($"呼び出したオブジェクト:{this.gameObject}");
        // 人数更新
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount + 1;
        Debug.Log($"現在のルーム参加人数変更:{_roomInfo.CurrentParticipantCount}");
        // 入室時の仮の名前
        string name = "";
        if (_roomInfo.CurrentParticipantCount == 1)
        {

            name = $"{_roomInfo.CurrentParticipantCount}_Host";
            Debug.Log($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        else
        {

            name = $"{_roomInfo.CurrentParticipantCount}_Client";
            Debug.Log($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        
        NetworkObject participant = _networkRunner.GetPlayerObject(playerRef);
         INameMemory iNameMemory = participant.GetComponent< INameMemory>();
        if (iNameMemory == null)
        {

            Debug.LogError($"名前を設定するためのインターフェースが見つかりません");

        }
        iNameMemory.Name = name;
        iNameMemory.RPC_NameUpdate();
        int index = _roomInfo.SetName(name);
        // テキスト設定
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Character = name;
        // テキスト更新
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();
        Debug.Log($"参加者加算処理＿終了: {this.GetType().Name}クラス");

    }

    /// <summary>
    /// 参加者が減った時にルーム管理から削除
    /// 呼び出し:ホストとクライアント
    /// 実行する:ホスト
    /// </summary>
    public async void ParticipantRemove(PlayerRef playerRef)
    {

        await GetRoomAwait();
        // 名前を消したあとの名前の配列をもらう
        string[] nameInfos = _roomInfo.RemoveName(_networkRunner.GetPlayerObject(playerRef).name);
        for (int i = 0; i < nameInfos.Length; i++)
        {

            // ヒエラルキー上にあるオブジェクト指定
            int index = i + 1;
            // テキスト設定
            GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Character = $"{nameInfos[i]}";
            // テキスト更新
            GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();

        }
        // 更新
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount - 1;
        Debug.Log($"現在のルーム参加人数変更:{_roomInfo.CurrentParticipantCount}");

    }

}
