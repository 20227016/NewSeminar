using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.UI;

/// <summary>
/// PlayerResurrection.cs
/// クラス説明
/// プレイヤー蘇生
///
/// 作成日: 9/26
/// 作成者: 山田智哉
/// </summary>
public class PlayerResurrection : IResurrection
{
    // キャンセレーショントークン
    private CancellationTokenSource _cancellationTokenSource = default;

    public void CancelResurrection(Transform thisTransform)
    {
        _cancellationTokenSource?.Cancel();
    }

    public async void Resurrection(Transform thisTransform, float resurrectionTime)
    {
        // 前の処理が残っていればキャンセル
        _cancellationTokenSource?.Cancel();

        // 新しいキャンセルトークンの作成
        _cancellationTokenSource = new CancellationTokenSource();

        // 自分の周囲を取得
        BoxCastStruct _boxcastStruct = BoxcastSetting(thisTransform);
        RaycastHit[] hits = Search.Sort(Search.BoxCastAll(_boxcastStruct));

        foreach (RaycastHit hit in hits)
        {
            // 自分を除外
            if (hit.collider.transform.name == thisTransform.name)
            {
                continue;
            }

            // 対象のキャラクターの CharacterBase を取得
            CharacterBase targetCharacter = hit.collider.transform.GetComponent<CharacterBase>();

            // CharacterBase が null であれば処理を中断
            if (targetCharacter == null)
            {
                continue;
            }

            IReceiveHeal receiveHeal = hit.collider.transform.GetComponent<IReceiveHeal>();

            if(receiveHeal == null)
            {
                continue;
            }

            // 対象のキャラクターがDEATH状態か確認
            if (targetCharacter.CurrentState == CharacterStateEnum.DEATH)
            {
                Debug.Log("蘇生開始");
                try
                {
                    // (resurrectionTime * 1000)ミリ秒待機
                    await UniTask.Delay((int)(resurrectionTime), cancellationToken: _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                // 蘇生完了の処理
                targetCharacter.ReceiveHeal((int)targetCharacter._characterStatusStruct._playerStatus.MaxHp);
            }

            break;
        }
    }

    /// <summary>
    /// ボックスキャスト設定
    /// </summary>
    /// <param name="transform">自分自身のトランスフォーム</param>
    /// <returns></returns>
    private BoxCastStruct BoxcastSetting(Transform transform)
    {
        return new BoxCastStruct
        {
            _originPos = transform.position,
            _size = transform.localScale * 3,
            _direction = transform.forward,
            _quaternion = Quaternion.identity,
            _layerMask = 1 << 6
        };
    }
}