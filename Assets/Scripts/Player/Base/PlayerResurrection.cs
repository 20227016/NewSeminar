using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

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

    public async void Resurrection(Transform thisTransform, float resurrectionTime)
    {
        // 前のキャンセル処理が残っていればキャンセル
        _cancellationTokenSource?.Cancel();

        // 新しいキャンセルトークンの作成
        _cancellationTokenSource = new CancellationTokenSource();

        BoxCastStruct _boxcastStruct = BoxcastSetting(thisTransform);
        RaycastHit[] hits = Search.Sort(Search.BoxCastAll(_boxcastStruct));

        foreach (RaycastHit hit in hits)
        {
            // 自分を除外
            if (hit.collider.transform != thisTransform)
            {
                // 対象のキャラクターの CharacterBase を取得
                CharacterBase targetCharacter = hit.collider.transform.GetComponent<CharacterBase>();

                // CharacterBase が null であれば処理を中断
                if (targetCharacter == null)
                {
                    Debug.LogWarning("ターゲットに CharacterBase コンポーネントが存在しません");
                    return;
                }


                // 対象のキャラクターがDEATH状態か確認
                if (targetCharacter._currentState == CharacterStateEnum.DEATH)
                {
                    Debug.Log("蘇生開始" + targetCharacter.name);

                    try
                    {
                        // (resurrectinTime * 1000)ミリ秒待機
                        await UniTask.Delay((int)(resurrectionTime * 1000), cancellationToken: _cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Log("蘇生キャンセル" + targetCharacter.name);
                        return;
                    }

                    // 蘇生完了の処理
                    Debug.Log("蘇生完了" + targetCharacter.name);

                }

                break;
            }
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
            _size = transform.localScale,
            _direction = transform.forward,
            _quaternion = Quaternion.identity,
            _layerMask = 1 << 6
        };
    }
}