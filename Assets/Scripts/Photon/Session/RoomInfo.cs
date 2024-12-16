using Fusion;
using UnityEngine;
/// <summary>
/// ネットワークで使用する変数の管理
/// 現在の役割
/// ・RoomInfoで現在の部屋の人数をCurrentPlayerCountに代入する(同期変数)
/// </summary>
public class RoomInfo : NetworkBehaviour
{

    [Networked]
    public string RoomName { get; set; } = "Room1";
    // 現在のプレイヤー数[ネットワーク上で同期]
    [Networked]
    public int CurrentParticipantCount { get; set; } = 0 ;
    // 最大プレイヤー数[ネットワーク上で同期]
    [Networked]
    public int MaxParticipantCount { get; set; } = 4;

    /// <summary>
    /// 参加者の名前
    /// </summary>
    private (string name, bool isRegistration)[] _nameInfos = new (string name, bool isRegistration)[] { ("名前１",false),("名前２", false),("名前３", false),("名前４",false) };

    /// <summary>
    /// 名前をセットしセットしたインデックスを補正した値を返す
    /// 補正はヒエラルキー上にあるオブジェクトの名前を指定するため
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int SetName(string name)
    {

        for (int i = 0; i < _nameInfos.Length; i++)
        {

            // 名前が登録済みか
            if (_nameInfos[i].isRegistration)
            {

                continue;

            }
            _nameInfos[i] = (name,true);
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
    public (string name, bool isRegistration)[] RemoveName(string name)
    {

        for (int i = 0; i < _nameInfos.Length; i++)
        {

            // 名前が一致するか
            if (_nameInfos[i].name != name)
            {

                continue;

            }
            Debug.LogError("削除対象発見");
            // ヒエラルキー上にあるオブジェクトとそろえる
            int index = i + 1;
            _nameInfos[i] = ($"名前{index}", false);
            break;

        }
        Sort();
        return _nameInfos;

    }

    public int ReName(string newName, string targetName)
    {

        Debug.LogError("改名");
        Debug.LogError($"名前スペース数{_nameInfos.Length}");
        Debug.LogError($"元の名前：{targetName}新たな名前：{newName}");

        //for (int i = 1; i < 5; i++)
        //{

        //    // テキスト設定
        //    string memoryName = GameObject.Find($"Participant_{i}").GetComponent<TextMemory>().Name;


        //}
        for (int i = 0; i < _nameInfos.Length; i++)
        {

            // 名前が一致するか
            if (_nameInfos[i].name != targetName)
            {

                continue;

            }
            Debug.LogError("改名対象発見");
            _nameInfos[i].name = newName;
            Debug.LogError($"ループイントと{i}");
            // ヒエラルキー上にあるオブジェクトとそろえる
            int index = i + 1;
            return index;

        }
        Debug.LogError("改名対象未発見");
        return 0;

    }

    /// <summary>
    /// 隙間を埋める
    /// </summary>
    public void Sort()
    {
        Debug.LogError("ソート");
        for (int i = 1; i < _nameInfos.Length; i++)
        {

            /*   今のインデックスが中身があるかつ前のインデックスが空の時に通る*/
            if (!_nameInfos[i].isRegistration)
            {

                continue;

            }
            if (_nameInfos[i-1].isRegistration)
            {

                continue;

            }
            // 穴を埋める（１人ずつ通るので複数個所穴が開いていることはない）
            _nameInfos[i - 1] = _nameInfos[i];
            // ヒエラルキー上にあるオブジェクトとそろえる
            int index = i + 1;
            _nameInfos[i] = ($"名前{index}", false);
            break;

        }

    }

}