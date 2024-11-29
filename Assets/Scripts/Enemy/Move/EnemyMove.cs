
using UnityEngine;
using System.Collections;

/// <summary>
/// EnemyMove.cs
/// クラス説明
/// 敵の移動
///
/// 作成日: 9/12
/// 作成者: 高橋光栄
/// </summary>
public class EnemyMove : IEnemyMove
{

    /// <summary>
    /// 移動処理
    /// </summary>
    /// <param name="moveDirection">移動方向</param>
    /// <param name="speed">プレイヤーのステータス</param>
    public void Execute(Vector3 startPos, Vector3 targetPos, float speed)
    {

        // 方向を求める
        float right = targetPos.x - startPos.x;
        float forward = targetPos.z - startPos.z;
        Vector3 director = Vector3.right * right +
                           Vector3.up * startPos.y + 
                           Vector3.forward * forward;
        


    }

}