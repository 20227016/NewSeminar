using Fusion;
using System.Threading.Tasks;
using UnityEngine;


public class BaseRoom : NetworkBehaviour
{

    /// <summary>
    /// 部屋情報
    /// </summary>
    protected RoomInfo _roomInfo = default;

    /// <summary>
    /// ネットワークランナー
    /// </summary>
    protected NetworkRunner _networkRunner = default;

    /// <summary>
    /// 自分のネットワークオブジェクトコンポーネント
    /// </summary>
    protected NetworkObject _networkObject = default;

    public override void Spawned()
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
        _roomInfo = GameObject.Find("Room").GetComponent<RoomInfo>();
        if (_roomInfo == null)
        {

            Debug.LogError($"{this.gameObject}でルーム管理クラスが見つかりません");

        }
        // データ初期化
        _roomInfo.RoomName = _roomInfo.RoomName;
        _roomInfo.MaxParticipantCount = _roomInfo.MaxParticipantCount;
        _roomInfo.CurrentParticipantCount = _roomInfo.CurrentParticipantCount;

    }

    /// <summary>
    /// _roomInfo取得まで待機
    /// </summary>
    /// <returns></returns>
    protected async Task GetRoomAwait()
    {

        while (_roomInfo == null)
        {

            await Task.Delay(100); // 100msごとにチェック

        }

    }

}
