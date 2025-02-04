
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

    // 回避開始までの遅延(ミリ秒)
    private const int AVOIDANCE_START_DELAY = 100;

    public void Avoidance(Transform transform, Rigidbody rigidbody, Vector2 avoidanceDirection, float avoidanceDistance, float avoidanceDuration)
    {
        if (_isAvoiding) return;

        Vector3 normalizedAvoidanceDirection = new Vector3(avoidanceDirection.x, 0, avoidanceDirection.y).normalized;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + normalizedAvoidanceDirection * avoidanceDistance;

        AvoidanceCoroutine(rigidbody, transform, startPosition, endPosition, avoidanceDuration, normalizedAvoidanceDirection).Forget();
    }

    private async UniTaskVoid AvoidanceCoroutine(Rigidbody rigidbody,Transform transform, Vector3 startPosition, Vector3 endPosition, float duration, Vector3 moveDirection)
    {
        _isAvoiding = true;
        await UniTask.Delay(AVOIDANCE_START_DELAY);

        float elapsedTime = 0f;
        while (elapsedTime < duration - (AVOIDANCE_START_DELAY * 0.001))
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, progress);

            if (Physics.Raycast(transform.position, moveDirection, transform.localScale.x / 2f, LayerMask.GetMask("Stage")))
            {
                break;
            }

            rigidbody.MovePosition(nextPosition);
            await UniTask.Yield();
        }

        _isAvoiding = false;
    }

}