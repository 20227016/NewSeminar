
using Fusion;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RoomCreation.cs
/// クラス説明
/// ルーム作成
///
/// 作成日: /
/// 作成者: 
/// </summary>
public class RoomCreation : MonoBehaviour,IClickEvent
{
    [SerializeField]
    private RoomManager_old _roomManager;
    public void StringClickEventAction(string roomName)
    {
        RoomCreationCall(roomName);
    }

    /// <summary>
    /// ルーム作成処理を呼び出す
    /// </summary>
    private void RoomCreationCall(string roomName)
    {
        _roomManager.CreateRoom(roomName);
    }

    public void VoidClickEventAction() { }
    public void NetworkClickEventAction(NetworkRunner runner, List<SessionInfo> sessionList) { }
}