using Fusion;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// ネットワークで使用する変数の管理
/// 現在の役割
/// ・RoomInfoで現在の部屋の人数をCurrentPlayerCountに代入する(同期変数)
/// </summary>
public class RoomInfo : NetworkBehaviour
{

    [Networked]
    public string RoomName { get; set; } = "Room(ya-ya-ya-ya-ya-)";
    // 現在のプレイヤー数[ネットワーク上で同期]
    [Networked]
    public int CurrentParticipantCount { get; set; } = 0;
    // 最大プレイヤー数[ネットワーク上で同期]
    [Networked]
    public int MaxParticipantCount { get; set; } = 4;

    /// <summary>
    /// 参加者の状態
    /// </summary>
    private (string name, bool isRegistration , bool isReady)[] _participationInfos = new (string name, bool isRegistration, bool isReady)[] 
                                                                             { ("名前1",false,false),("名前2", false,false),("名前3", false,false),("名前4",false,false) };

    /// <summary>
    /// 名前をセットしセットしたインデックスを補正した値を返す
    /// 補正はヒエラルキー上にあるオブジェクトの名前を指定するため
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int SetName(string name)
    {

        for (int i = 0; i < _participationInfos.Length; i++)
        {

            // 名前が登録済みか
            if (_participationInfos[i].isRegistration)
            {

                continue;

            }
            _participationInfos[i] = (name,true,_participationInfos[i].isReady);
            // ヒエラルキー上にあるオブジェクトとそろえる
            int index = i + 1;
            return index;

        }
        return 0;

    }

    /// <summary>
    /// 名前を削除
    /// </summary>
    /// <param name="name"></param>
    public string[] RemoveName(string name)
    {

        List<string> returnNames = new();
        for (int i = 0; i < _participationInfos.Length; i++)
        {

            // 名前が一致するか
            if (_participationInfos[i].name != name)
            {

                continue;

            }
            // ヒエラルキー上にあるオブジェクトとそろえる
            int index = i + 1;
            _participationInfos[i] = ($"名前{index}", false, false);
            break;

        }
        Sort();
        foreach ((string name, bool isRegistration, bool isReady) nameInfo in _participationInfos)
        {

            returnNames.Add(nameInfo.name);

        }
        return returnNames.ToArray() ;

    }

    /// <summary>
    /// 隙間を埋める
    /// </summary>
    public void Sort()
    {

        Debug.Log("ソート");
        for (int i = 1; i < _participationInfos.Length; i++)
        {

            /*   今のインデックスが中身があるかつ前のインデックスが空の時に通る*/
            if (!_participationInfos[i].isRegistration || _participationInfos[i - 1].isRegistration)
            {

                continue;

            }
            // 穴を埋める（１人ずつ通るので複数個所穴が開いていることはない）
            _participationInfos[i - 1] = _participationInfos[i];
            // ヒエラルキー上にあるオブジェクトとそろえる
            int index = i + 1;
            _participationInfos[i] = ($"名前{index}", false, false);

        }

    }

    /// <summary>
    /// 名前の変更
    /// </summary>
    /// <param name="newName"></param>
    /// <param name="targetName"></param>
    /// <returns></returns>
    public int ReName(string newName, string targetName)
    {

        for (int i = 0; i < _participationInfos.Length; i++)
        {

            // 名前が一致するか
            if (_participationInfos[i].name != targetName)
            {

                continue;

            }
            _participationInfos[i].name = newName;
            // ヒエラルキー上にあるオブジェクトとそろえる
            int index = i + 1;
            return index;

        }
        return 0;

    }

    /// <summary>
    /// 準備状態を変える
    /// </summary>
    public void ChangeReady(string targetName)
    {

        for (int i = 0; i < _participationInfos.Length; i++)
        {

            // 名前が一致するか
            if (_participationInfos[i].name != targetName)
            {

                continue;

            }
            _participationInfos[i].isReady = !_participationInfos[i].isReady;
            Debug.Log($"{_participationInfos[i].name}の準備を{_participationInfos[i].isReady}に");

        }

    }

    public int ReadyGoCount()
    {

        int num = default;
        foreach ((string name, bool isRegistration, bool isReady) nameInfo in _participationInfos)
        {

            if (nameInfo.isReady)
            {

                num++;

            }

        }

        return num;

    }

    /// <summary>
    /// 出撃できるかの確認
    /// </summary>
    public bool GoCheck()
    {

        // 参加している人がすべて準備完了中の時
        for (int i = 0; i < CurrentParticipantCount; i++)
        {

            if (_participationInfos[i].isReady)
            {

                continue;

            }
            return false;

        }
        return true;

    }

}