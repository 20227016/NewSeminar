
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// Avoidance.cs
/// クラス説明
/// プレイヤー回避
///
/// 作成日: 9/10
/// 作成者: 高橋光栄
/// </summary>
public class PlayerAvoidance : IAvoidance
{
    // 回避中フラグ
    private bool _isAvoiding = false;

    // 回避開始までの遅延
    private const int AVOIDANCE_START_DELAY = 100;

    public void Avoidance(Transform transform, Vector2 avoidanceDirection, float avoidanceDistance, float avoidanceDuration)
    {
        if (_isAvoiding) return;

        Vector3 normalizedAvoidanceDirection = new Vector3(avoidanceDirection.x, 0, avoidanceDirection.y).normalized;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + normalizedAvoidanceDirection * avoidanceDistance;

        AvoidanceCoroutine(transform, startPosition, endPosition, avoidanceDuration, normalizedAvoidanceDirection).Forget();
    }

    /// <summary>
    /// 回避処理の非同期コルーチン
    /// </summary>
    private async UniTaskVoid AvoidanceCoroutine(Transform transform, Vector3 startPosition, Vector3 endPosition, float duration, Vector3 moveDirection)
    {
        _isAvoiding = true;

        await UniTask.Delay(AVOIDANCE_START_DELAY); // 遅延時間をミリ秒単位で指定

        float elapsedTime = 0f;

        // 回避の持続時間が経過するまでループ
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // progress は回避処理の進行度を表す割合
            float progress = Mathf.Clamp01(elapsedTime / duration);

            // 移動先を計算
            Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, progress);

            // 移動方向にレイキャストを飛ばして壁があるか確認
            if (Physics.Raycast(transform.position, moveDirection, transform.localScale.x / 2f, LayerMask.GetMask("Stage")))
            {
                // 衝突している場合、回避を終了させる
                break;
            }

            // 壁に衝突していなければ位置を更新
            transform.position = nextPosition;

            // フレームごとに待機
            await UniTask.Yield();
        }

        _isAvoiding = false;
    }
    
}