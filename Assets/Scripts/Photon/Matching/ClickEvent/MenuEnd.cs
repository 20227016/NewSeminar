
using UnityEngine;
using System.Collections;
using Fusion;
using System.Collections.Generic;

/// <summary>
/// MenuEnd.cs
/// クラス説明
/// ゲーム終了
///
/// 作成日: 10/7
/// 作成者: 高橋光栄
/// </summary>
public class MenuEnd : MonoBehaviour,IClickEvent
{
    public void VoidClickEventAction()
    {
        print("ゲーム終了");
    }
    public void StringClickEventAction(string roomName) { }
    public void NetworkClickEventAction(NetworkRunner runner, List<SessionInfo> sessionList) { }
}