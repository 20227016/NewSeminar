
using UnityEngine;
using System.Collections;
using Fusion;

/// <summary>
/// FireMagic.cs
/// クラス説明
/// 魔法弾制御
/// 
/// 作成日: 12/11
/// 作成者: 石井直人
/// </summary>
public class FireMagic : BaseEnemy
{
    [Tooltip("魔法弾の速度")]
    [SerializeField] private float _speed = 10f;

    [Tooltip("魔法弾の生存時間")]
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
    /// 毎フレーム、魔法弾を移動させる。
    /// </summary>
    private void Update()
    {
        // 前方に移動させる
        transform.position += transform.forward * _speed * Time.deltaTime;

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

        if (other.gameObject.layer == 6 || other.gameObject.layer == 8)
        {
            // 衝突後に非アクティブ化
            Deactivate();
        }
    }

    /// <summary>
    /// 魔法弾を非アクティブ化する。
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

    public override void RPC_ReceiveDamage(int damegeValue)
    {

        Debug.Log("オーバーライド");
        Destroy(gameObject);

    }

}