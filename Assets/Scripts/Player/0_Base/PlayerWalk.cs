
using UnityEngine;

/// <summary>
/// PlayerMove.cs
/// クラス説明
/// プレイヤー歩行クラス
///
/// 作成日: 9/10
/// 作成者: 山田智哉
/// </summary>
public class PlayerWalk :IMove
{
    // 移動方向キャッシュ用
    private Vector3 _cachedMoveDirection = default;

    public void Move(Transform transform, Vector2 moveDirection, float moveSpeed, Rigidbody rigidbody)
    {
        // Vector2をVector3に変換
        _cachedMoveDirection.Set(moveDirection.x, 0, moveDirection.y);

        // 移動量を計算 
        Vector3 moveVector = _cachedMoveDirection * moveSpeed / 10; // 時間に基づいて移動量を計算

        // Rigidbodyを使って移動
        rigidbody.MovePosition(transform.position + moveVector);

        if (_cachedMoveDirection != Vector3.zero)
        {
            // 回転方向を計算し、Rigidbodyで回転を適用
            Quaternion targetRotation = Quaternion.LookRotation(_cachedMoveDirection);
            rigidbody.MoveRotation(targetRotation);
        }
    }
}
