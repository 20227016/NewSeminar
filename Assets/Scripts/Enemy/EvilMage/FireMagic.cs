
using UnityEngine;
using System.Collections;

/// <summary>
/// FireMagic.cs
/// クラス説明
/// 魔法弾制御
/// 
/// 作成日: 12/11
/// 作成者: 石井直人
/// </summary>
public class FireMagic : MonoBehaviour
{
    [Tooltip("魔法弾の速度")]
    [SerializeField] private float _speed = 10f;

    [Tooltip("魔法弾の生存時間")]
    [SerializeField] private float _lifeTime = 5f;

    [Tooltip("魔法弾のダメージ")]
    [SerializeField] private float _damage = 10f;

    private float _elapsedTime = 0f; // 経過時間
    private bool _isActive = true;  // アクティブ状態を管理

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
    /// 毎フレーム、魔法弾を移動させる。
    /// </summary>
    private void Update()
    {
        if (!_isActive) return;

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
    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;

        // ダメージを与える処理（例: プレイヤーなど特定のレイヤーの場合）
        if (other.CompareTag("Player")) // プレイヤーに対してダメージを与える
        {
            // プレイヤーのダメージ処理を呼び出す（仮の例）
            Debug.Log($"FireMagic Hit {other.gameObject.name}, dealt {_damage} damage.");

            // 衝突後に非アクティブ化
            Deactivate();
        }
        else if (other.gameObject.layer == 8)
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
        _isActive = false;
        gameObject.SetActive(false); // オブジェクトを非アクティブ化
        _elapsedTime = 0f;           // 経過時間をリセット
    }

    /// <summary>
    /// 魔法弾を再利用する際の初期化処理。
    /// </summary>
    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        _elapsedTime = 0f;
        _isActive = true;
        gameObject.SetActive(true); // オブジェクトをアクティブ化
    }
}