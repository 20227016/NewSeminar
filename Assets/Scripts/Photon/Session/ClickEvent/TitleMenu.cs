
using UnityEngine;
using System.Collections;
using Fusion;
using System.Collections.Generic;

/// <summary>
/// MeneStart.cs
/// クラス説明
/// ゲーム参加
///
/// 作成日: /
/// 作成者: 
/// </summary>
public class TitleMenu : MonoBehaviour
{
    [SerializeField, Tooltip("タイトル画面")]
    private GameObject _titleSpace = default;

    [SerializeField, Tooltip("参加画面")]
    private GameObject _roomSpace = default;

    /// <summary>
    /// マッチング開始
    /// </summary>
    public void Matching()
    {

        print("ゲームに参加");
        _titleSpace.SetActive(false);
        _roomSpace.SetActive(true);

    }

    public void EndGame()
    {
        print("ゲーム終了");
    }

}