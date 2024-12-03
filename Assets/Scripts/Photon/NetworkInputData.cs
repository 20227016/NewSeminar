
using UnityEngine;
using Fusion;

/// <summary>
/// NetworkInputData.cs
/// クラス説明
/// ネットワーク入力情報構造体
///
/// 作成日: 9/3
/// 作成者: 山田智哉
/// </summary>
public struct NetworkInputData : INetworkInput
{

    public Vector2 _moveInput;

    public NetworkButtons _buttons;

    public enum NetworkInputButtons
    {
        ATTACK,

    }
    
}