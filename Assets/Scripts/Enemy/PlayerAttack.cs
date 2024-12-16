
using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerAttack.cs
/// クラス説明
///
///
/// 作成日: /
/// 作成者: 
/// </summary>
public class PlayerAttack : MonoBehaviour
{

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {

    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        // プレイヤーの前方向にRayを飛ばす
        RaycastHit hit;
        Vector3 rayStart = transform.position; // Rayの開始位置
        Vector3 rayDirection = transform.forward; // Rayの方向
        float rayDistance = 15f; // Rayの距離

        // Rayを視覚化
        Debug.DrawRay(rayStart, rayDirection * rayDistance, Color.red, 1.0f); // 赤色のRayを1秒間表示

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Physics.Raycast(rayStart, rayDirection, out hit, rayDistance))
            {
                Debug.Log($"攻撃対象: {hit.collider.gameObject.name}");
                Attack(hit.collider.gameObject); // ヒットしたオブジェクトをターゲットに
            }
            else
            {
                Debug.Log("攻撃対象が見つかりませんでした");
            }
        }
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private void Attack(GameObject target)
    {
        if (target.TryGetComponent<Golem>(out Golem golem))
        {
            golem.OnPlayerAttack(transform); // ゴーレムに攻撃通知を送る
        }
        else
        {
            Debug.Log("攻撃対象はゴーレムではありません");
        }
    }
}