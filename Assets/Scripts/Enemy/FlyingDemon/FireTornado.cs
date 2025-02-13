
using UnityEngine;
using Fusion;
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

    //AudioSource型の変数を宣言
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip型の変数を宣言
    [SerializeField] private AudioClip _sE = default;

    /// <summary>
    /// 効果音発生
    /// </summary>
    private void Start()
    {
        _audioSource.PlayOneShot(_sE);
    }

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
        Destroy(gameObject);
        // gameObject.SetActive(false); // オブジェクトを非アクティブ化
    }

    /// <summary>
    /// HPが0以下になったら呼ばれる処理(Base参照)
    /// </summary>
    protected override void OnDeath()
    {

    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damegeValue">ダメージ</param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public override void RPC_ReceiveDamage(int damegeValue)
    {

    }

}
