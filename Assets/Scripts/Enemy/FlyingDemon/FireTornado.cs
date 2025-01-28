
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

public class FireTornado : BaseEnemy
{
    [Tooltip("炎の竜巻の生存時間")]
    [SerializeField] private float _lifeTime = 5f;

    private float _elapsedTime = 0f; // 経過時間

    /// <summary>
    /// 炎の竜巻の生存処理
    /// </summary>
    private void Update()
    {
        // 寿命を超えた場合、非アクティブ化
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _lifeTime)
        {
            Deactivate();
        }
    }

    /// <summary>
    /// 他のオブジェクトと衝突した際の処理。
    /// </summary>
    /// <param name="collision">衝突情報</param>
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    /// <summary>
    /// 炎の竜巻を非アクティブ化する。
    /// </summary>
    private void Deactivate()
    {
        gameObject.SetActive(false); // オブジェクトを非アクティブ化
    }

    /// <summary>
    /// HPが0以下になったら呼ばれる処理(Base参照)
    /// </summary>
    protected override void OnDeath()
    {

    }
}
