using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingAttack : MonoBehaviour
{
    [Tooltip("翼のダメージ")]
    [SerializeField] private float _damage = 10f;

    /// <summary>
    /// 他のオブジェクトと衝突した際の処理。
    /// </summary>
    /// <param name="collision">衝突情報</param>
    private void OnTriggerEnter(Collider other)
    {
        // ダメージを与える処理（例: プレイヤーなど特定のレイヤーの場合）
        if (other.CompareTag("Player")) // プレイヤーに対してダメージを与える
        {
            // プレイヤーのダメージ処理を呼び出す（仮の例）
            Debug.Log($"Wing Hit {other.gameObject.name}, dealt {_damage} damage.");
        }
    }
}
