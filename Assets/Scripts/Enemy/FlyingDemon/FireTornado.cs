
using UnityEngine;
using System.Collections;

/// <summary>
/// FireTornado.cs
/// クラス説明
/// 炎の竜巻制御
/// 
/// 作成日: 1/17
/// 作成者: 石井直人
/// </summary>

public class FireTornado : MonoBehaviour
{
    [Tooltip("炎の竜巻の生存時間")]
    [SerializeField] private float lifeTime = 5f;

    [Tooltip("炎の竜巻のダメージ")]
    [SerializeField] private float damage = 20f;

    private float _elapsedTime = 0f; // 経過時間
    private bool _isActive = true;  // アクティブ状態を管理

    /// <summary>
    /// 炎の竜巻の生存処理
    /// </summary>
    private void Update()
    {
        if (!_isActive) return;

        // 寿命を超えた場合、非アクティブ化
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= lifeTime)
        {
            Deactivate();
        }
    }

    /// <summary>
    /// 他のオブジェクトと衝突した際の処理。
    /// </summary>
    /// <param name="collision">衝突情報</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;

        // ダメージを与える処理（例: プレイヤーなど特定のレイヤーの場合）
        if (other.CompareTag("Player")) // プレイヤーに対してダメージを与える
        {
            // プレイヤーのダメージ処理を呼び出す（仮の例）
            Debug.Log($"Hit {other.gameObject.name}, dealt {damage} damage.");
        }
    }

    /// <summary>
    /// 炎の竜巻を非アクティブ化する。
    /// </summary>
    private void Deactivate()
    {
        _isActive = false;
        gameObject.SetActive(false); // オブジェクトを非アクティブ化
        _elapsedTime = 0f;           // 経過時間をリセット
    }
}
