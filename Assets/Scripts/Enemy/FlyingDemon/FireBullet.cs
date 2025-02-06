
using UnityEngine;
using System.Collections;

/// <summary>
/// FireBullet.cs
/// クラス説明
/// 火球制御
/// 
/// 作成日: 1/17
/// 作成者: 石井直人
/// </summary>
public class FireBullet : BaseEnemy
{
    [Tooltip("火球の速度")]
    [SerializeField] private float _speed = 7.5f;

    [Tooltip("火球の生存時間")]
    [SerializeField] private float _lifeTime = 5f;

    private float _elapsedTime = 0f; // 経過時間

    [SerializeField] private GameObject _fireTornadoPrefab = default;

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
    /// 毎フレーム、火球を移動させる。
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

        if (other.gameObject.layer != 6 && other.gameObject.layer != 7)
        {
            // 衝突点を取得
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                // 炎の竜巻を生成
                Instantiate(
                    _fireTornadoPrefab,
                    hit.point,              // 衝突した表面の位置
                    Quaternion.Euler(0, 0, 0) // 回転を (0, 0, 0) に固定
                );
            }

            // 衝突後に非アクティブ化
            Deactivate();
        }
    }

    /// <summary>
    /// 火球を非アクティブ化する。
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