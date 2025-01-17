
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
    [Tooltip("魔法弾の生存時間")]
    [SerializeField] private float lifeTime = 5f;

    [Tooltip("魔法弾のダメージ")]
    [SerializeField] private float damage = 10f;

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
    /// 炎の竜巻を非アクティブ化する。
    /// </summary>
    private void Deactivate()
    {
        _isActive = false;
        gameObject.SetActive(false); // オブジェクトを非アクティブ化
        _elapsedTime = 0f;           // 経過時間をリセット
    }
}
