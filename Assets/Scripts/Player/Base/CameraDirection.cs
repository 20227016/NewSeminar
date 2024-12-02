
using UnityEngine;

/// <summary>
/// CameraDirection.cs
/// クラス説明
///
///
/// 作成日: 9/13
/// 作成者: 山田智哉
/// </summary>
public class CameraDirection
{
    // メインカメラのトランスフォーム
    private Transform _cameraTransform;

    public CameraDirection(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform;
    }

    /// <summary>
    /// カメラ基準の移動方向を計算
    /// </summary>
    /// <param name="inputDirection">入力方向</param>
    /// <returns>カメラ基準の移動方向</returns>
    // カメラ基準の移動方向を取得
    public Vector2 GetCameraRelativeMoveDirection(Vector2 inputDirection)
    {
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;

        forward.y = 0; // 高さを無視
        right.y = 0;   // 高さを無視

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * inputDirection.y + right * inputDirection.x;
        return new Vector2(moveDirection.x, moveDirection.z);
    }
}