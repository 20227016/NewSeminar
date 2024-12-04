
using UnityEngine;
using System.Collections;
using Fusion;
using System.Collections.Generic;

/// <summary>
/// MeneStart.cs
/// クラス説明
/// ゲームスタート
///
/// 作成日: /
/// 作成者: 
/// </summary>
public class MenuStart : MonoBehaviour,IClickEvent
{
    [SerializeField, Tooltip("1入れる")]
    private GameObject _title1 = default;

    [SerializeField, Tooltip("2入れる")]
    private GameObject _title2 = default;

    // ゲームをスタートする
    public void VoidClickEventAction()
    {
        print("ゲームをスタートします");
        _title1.SetActive(false);
        _title2.SetActive(true);
    }
    public void StringClickEventAction(string roomName) { }
    public void NetworkClickEventAction(NetworkRunner runner, List<SessionInfo> sessionList) { }
}