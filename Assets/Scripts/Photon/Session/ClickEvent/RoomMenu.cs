using Fusion;
using System.Collections;
using System.Collections.Generic;
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

        string newName = _nameInputField.text;
        // 自分のセッションのプレイヤーレフ
        PlayerRef playerRef = _networkRunner.LocalPlayer;
        // 自分のセッションの参加者オブジェクト
        NetworkObject participantsObj = _networkRunner.GetPlayerObject(playerRef);
        Debug.LogError($"プレイヤーレフとの関連オブジェ{participantsObj}");
        IRoomController _iRoomController = participantsObj.GetComponent<IRoomController>();
        if (_iRoomController == null)
        {

            Debug.LogError("ホストオブジェクトにルーム情報を渡すインターフェースが見つかりません");

        }
        _iRoomController.RPC_ParticipantReName(newName, participantsObj);

    }

}
