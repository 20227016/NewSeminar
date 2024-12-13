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

    public  void Start()
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
        if(_networkRunner == null)
        {

            Debug.LogError($"自分についているネットワークランナーが見つかりません");

        }

    }

    /// <summary>
    /// 参加者が増えたときにルーム管理に追加
    /// 呼び出し:ホストとクライアント
    /// 実行する:ホスト
    /// </summary>
    public async void ParticipantCountAdd(PlayerRef playerRef)
    {

        Debug.Log($"参加者加算処理＿開始: {this.GetType().Name}クラス");
        // Roomクラス取得まで待つ
        await GetRoomAwait();
        Debug.Log($"呼び出したオブジェクト:{this.gameObject}");
        // 更新
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount + 1;
        Debug.Log($"現在のルーム参加人数変更:{_roomInfo.CurrentParticipantCount}");
        string text = "";
        if (_roomInfo.CurrentParticipantCount == 1)
        {

            text = $"{_networkRunner.GetPlayerObject(playerRef).name}";
            Debug.LogError($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        else
        {

            text = $"{_networkRunner.GetPlayerObject(playerRef).name}";
            Debug.LogError($"{_networkRunner.GetPlayerObject(playerRef).name}");

        }
        // テキスト設定
        GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<TextMemory>().Character = text;
        // テキスト更新
        GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<TextMemory>().RPC_TextUpdate();
        Debug.Log($"参加者加算処理＿終了: {this.GetType().Name}クラス");
    }

    /// <summary>
    /// 参加者が減った時にルーム管理から削除
    /// 呼び出し:ホストとクライアント
    /// 実行する:ホスト
    /// </summary>
    public async void ParticipantCountRemove(PlayerRef playerRef)
    {

        await GetRoomAwait();
        Debug.Log($"呼び出したオブジェクト:{this.gameObject}");
        // テキスト設定
        GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<TextMemory>().Character = $"名前";
        // テキスト更新
        GameObject.Find($"Participant_{_roomInfo.CurrentParticipantCount}").GetComponent<TextMemory>().RPC_TextUpdate();
        // 更新
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount - 1;
        Debug.Log($"現在のルーム参加人数変更:{_roomInfo.CurrentParticipantCount}");

    }

    /// <summary>
    /// 
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
