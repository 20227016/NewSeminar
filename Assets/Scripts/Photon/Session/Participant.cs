using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class Participant : NetworkBehaviour, IRoomController
{

   

    /// <summary>
    /// 部屋情報
    /// </summary>
    private RoomInfo _roomInfo = default;

    /// <summary>
    /// ネットワークランナー
    /// </summary>
    private NetworkRunner _networkRunner = default;

    /// <summary>
    /// 自分のネットワークオブジェクトコンポーネント
    /// </summary>
    private NetworkObject _networkObject = default;

    public void Start()
    {

        Debug.Log($"Start処理＿開始: {this.GetType().Name}クラス");
        InitialGet();
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

        }
        Debug.Log($"Start処理＿終了: {this.GetType().Name}クラス");

    }

    /// <summary>
    /// 初期に取得する処理
    /// </summary>
    private void InitialGet()
    {

        _networkObject = this.GetComponent<NetworkObject>();
        if (_networkObject == null)
        {

            Debug.LogError($"自分についているネットワークオブジェクトが見つかりません");

        }
        _networkRunner = GetRunner.GetRunnerMethod();
        if (_networkRunner == null)
        {

            Debug.LogError($"自分についているネットワークランナーが見つかりません");

        }

    }

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
            Debug.LogError($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        else
        {

            name = $"{_roomInfo.CurrentParticipantCount}_Client";
            Debug.LogError($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        
        NetworkObject participant = _networkRunner.GetPlayerObject(playerRef);
         INameMemory iNameSet = participant.GetComponent< INameMemory>();
        if (iNameSet == null)
        {

            Debug.LogError($"名前を設定するためのインターフェースが見つかりません");

        }
        iNameSet.Name = name;
        iNameSet.RPC_NameUpdate();
        int index = _roomInfo.SetName(name);
        // テキスト設定
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Name = name;
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
        (string name, bool isRegistration)[] nameInfos = _roomInfo.RemoveName(_networkRunner.GetPlayerObject(playerRef).name);
        for (int i = 0; i < nameInfos.Length; i++)
        {

            // ヒエラルキー上にあるオブジェクト指定
            int index = i + 1;
            // テキスト設定
            GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Name = $"{nameInfos[i].name}";
            // テキスト更新
            GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();

        }
        // 更新
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount - 1;
        Debug.Log($"現在のルーム参加人数変更:{_roomInfo.CurrentParticipantCount}");

    }

    /// <summary>
    /// 名前の変更
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void RPC_ParticipantReName(string newName, NetworkObject participantsObj)
    {

        if (!_networkRunner.IsServer)
        {

            return;

        }
        await GetRoomAwait();
        int index = _roomInfo.ReName(newName, participantsObj.name);
        Debug.Log($"{index}");

        // テキスト設定
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Name = $"{newName}";
        // テキスト更新
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();
    }

    /// <summary>
    /// _roomInfo取得まで待機
    /// </summary>
    /// <returns></returns>
    private async Task GetRoomAwait()
    {

        while (_roomInfo == null)
        {

            await Task.Delay(100); // 100msごとにチェック

        }


    }

}
