using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviour
{

    /// <summary>
    /// 名前の入力欄
    /// </summary>
    private InputField _nameInputField = default;

    /// <summary>
    /// ネットワークランナー
    /// </summary>
    private NetworkRunner _networkRunner = default;

    /// <summary>
    /// ローカルにおける参加者オブジェクト
    /// </summary>
    private NetworkObject _myParticipantObj = default;

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {

        _nameInputField = GameObject.Find("NameInputField").GetComponent<InputField>();
        if (_nameInputField == null)
        {

            Debug.LogError("名前の入力欄が見つかりません");

        }
        _networkRunner = GetRunner.GetRunnerMethod();
        if (_networkRunner == null)
        {

            Debug.LogError($"自分についているネットワークランナーが見つかりません");

        }

    }

    /// <summary>
    /// 名前の変更
    /// </summary>
    public void ReName()
    {

        // セッションに紐づけているプレイヤーレフ
        PlayerRef playerRef = _networkRunner.LocalPlayer;
        // 参加者オブジェクトを取得
        _myParticipantObj = _networkRunner.GetPlayerObject(playerRef);
        string newName = _nameInputField.text;
        Debug.LogError($"プレイヤーレフとの関連づけられているオブジェクト{_myParticipantObj}");
        IReName iRename = _myParticipantObj.GetComponent<IReName>();
        if (iRename == null)
        {

            Debug.LogError("ホストオブジェクトにルーム情報を渡すインターフェースが見つかりません");

        }
        iRename.RPC_ParticipantReName(newName, _myParticipantObj);

    }

    /// <summary>
    /// 準備完了またホストの時または出撃
    /// </summary>
    public void Ready()
    {

        // セッションに紐づけているプレイヤーレフ
        PlayerRef playerRef = _networkRunner.LocalPlayer;
        // 参加者オブジェクトを取得
        _myParticipantObj = _networkRunner.GetPlayerObject(playerRef);
        Debug.Log($"{_myParticipantObj.name}呼び出したオブジェクト");
        IReady iReady = _myParticipantObj.GetComponent<IReady>();
        if (iReady == null)
        {

            Debug.LogError("ホストオブジェクトにルーム情報を渡すインターフェースが見つかりません");

        }
        iReady.ParticipantReady(_myParticipantObj);

    }

}
