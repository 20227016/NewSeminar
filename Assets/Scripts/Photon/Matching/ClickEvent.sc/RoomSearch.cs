
using UnityEngine;
using System.Collections;
using Fusion;
using System.Collections.Generic;

/// <summary>
/// RoomSearch.cs
/// クラス説明
///
///
/// 作成日: /
/// 作成者: 高橋光栄
/// </summary>
public class RoomSearch : MonoBehaviour,IClickEvent
{
    [SerializeField]
    private RoomManager_old _roomManager;
    public void VoidClickEventAction()
    {
        _roomManager.SearchRooms();
    }

    public void StringClickEventAction(string roomName) { }
    public void NetworkClickEventAction(NetworkRunner runner, List<SessionInfo> sessionList) { }
}