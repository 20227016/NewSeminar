
using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerMoveProvider.cs
/// クラス説明
/// 移動インターフェースを渡すクラス
///
/// 作成日: 9/13
/// 作成者: 山田智哉
/// </summary>
public class PlayerMoveProvider : IMoveProvider
{
    public PlayerWalk _playerWalk = new PlayerWalk();

    public PlayerRun _playerRun = new PlayerRun();

    public IMove GetWalk()
    {
        return _playerWalk;
    }

    public IMove GetRun()
    {
        return _playerRun;
    }
}