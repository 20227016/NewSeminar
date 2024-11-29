
using UnityEngine;

public interface IMove
{
    /// <summary>
    /// 移動処理
    /// </summary>
    /// <param name="transform">移動対象のtransform</param>
    /// <param name="moveDirection">移動方向</param>
    /// <param name="moveSpeed">移動速度</param>
    void Move(Transform transform, Vector2 moveDirection, float moveSpeed, Rigidbody rigidbody);
}