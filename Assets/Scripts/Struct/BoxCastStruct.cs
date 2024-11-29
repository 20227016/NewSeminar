
using UnityEngine;
using System.Collections.Generic;


public struct BoxCastStruct
{

    /// <summary>
    /// 開始位置
    /// </summary>
    public Vector3 _originPos;
    /// <summary>
    /// 大きさ（半径）
    /// </summary>
    public Vector3 _size;
    /// <summary>
    /// 開始位置を中心とした方向
    /// </summary>
    public Vector3 _direction;
    /// <summary>
    /// ボックスを中心とした回転
    /// </summary>
    public Quaternion _quaternion;
    /// <summary>
    /// 距離
    /// </summary>
    public float _distance;
    /// <summary>
    /// 探索するタグ
    /// </summary>
    public string[] _tags;
    /// <summary>
    /// 探索するレイヤー
    /// </summary>
    public LayerMask _layerMask;

}


