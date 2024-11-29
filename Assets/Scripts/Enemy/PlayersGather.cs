
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// PlayersGather.cs
/// クラス説明
/// プレイヤーの集合管理クラス
///
/// 作成日: 10/1
/// 作成者: 高橋光栄
/// </summary>
public class PlayersGather : MonoBehaviour
{

    private Subject<Unit> OnPlayerGather = new Subject<Unit>();

    public IObservable<Unit> PlayerGather => OnPlayerGather;

    private void OnTriggerEnter(Collider hitCollider)
    {
        if(hitCollider.gameObject.layer == 6)
        {
            print("プレイヤーがポータルに全員集合しました");
            OnPlayerGather.OnNext(Unit.Default);
        }
    }

}