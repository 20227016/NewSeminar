using UnityEngine;
using UniRx;
using System;
using Fusion;

/// <summary>
/// プレイヤー集合管理クラス
/// </summary>
public class PlayersGather : NetworkBehaviour
{
    private int _playersInPortal = 0;       // サークル内のプレイヤー数
    private const int _requiredPlayers = 4; // 必要なプレイヤー数

    private Subject<Unit> OnPlayerGather = new Subject<Unit>();
    public IObservable<Unit> PlayerGather => OnPlayerGather;

    private void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.gameObject.layer == 6)
        {
            if (Object.HasStateAuthority) // サーバーでカウントする
            {
                _playersInPortal++;
                CheckPlayerGatherCondition();
            }
        }
    }

    private void OnTriggerExit(Collider hitCollider)
    {
        if (hitCollider.gameObject.layer == 6)
        {
            if (Object.HasStateAuthority)
            {
                _playersInPortal--;
            }
        }
    }

    private void CheckPlayerGatherCondition()
    {
        if (_playersInPortal >= _requiredPlayers)
        {
            // プレイヤーが揃ったらイベント発火
            OnPlayerGather.OnNext(Unit.Default);
        }
    }
}
