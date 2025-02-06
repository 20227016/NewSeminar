
using UnityEngine;

/// <summary>
/// PlayerRun.cs
/// クラス説明
/// プレイヤーの走行クラス
///
/// 作成日: 9/13
/// 作成者: 山田智哉
/// </summary>
public class PlayerRun : IMove
{
    // 移動方向キャッシュ用
    private Vector3 _cachedMoveDirection = default;
    private const float RaycastDistance = 0.1f; // レイの距離（調整可能）

    public void Move(Transform transform, Vector2 moveDirection, float moveSpeed, Rigidbody rigidbody)
    {
        // Vector2をVector3に変換
        _cachedMoveDirection.Set(moveDirection.x, 0, moveDirection.y);

        // レイキャストで進行方向をチェック
        if (IsBlockedByStage(transform.position + new Vector3(0, 1, 0), _cachedMoveDirection))
        {
            Debug.Log("キャンセル");
            return; // 進行方向に壁がある場合、移動しない
        }

        // 移動量を計算
        Vector3 moveVector = _cachedMoveDirection * moveSpeed / 10;

        // Rigidbodyを使って移動
        rigidbody.MovePosition(transform.position + moveVector);

        if (_cachedMoveDirection != Vector3.zero)
        {
            // 回転方向を計算し、Rigidbodyで回転を適用
            Quaternion targetRotation = Quaternion.LookRotation(_cachedMoveDirection);
            rigidbody.MoveRotation(targetRotation);
        }
    }

    /// <summary>
    /// 進行方向に Stage レイヤーのオブジェクトがあるかチェック
    /// </summary>
    private bool IsBlockedByStage(Vector3 position, Vector3 direction)
    {
        int stageLayerMask = LayerMask.GetMask("Stage");

        // プレイヤーの中心から進行方向へレイを飛ばす
        return Physics.Raycast(position, direction, RaycastDistance, stageLayerMask);
    }
}