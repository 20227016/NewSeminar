
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
public class FireBullet : MonoBehaviour
{
    [Tooltip("火球の速度")]
    [SerializeField] private float speed = 7.5f;

    [Tooltip("火球の生存時間")]
    [SerializeField] private float lifeTime = 5f;

    [Tooltip("火球のダメージ")]
    [SerializeField] private float damage = 15f;

    private float _elapsedTime = 0f; // 経過時間
    private bool _isActive = true;  // アクティブ状態を管理

    [SerializeField] private GameObject _fireTornadoPrefab = default;

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
    /// 毎フレーム、火球を移動させる。
    /// </summary>
    private void Update()
    {
        if (!_isActive) return;

        // 前方に移動させる
        transform.position += transform.forward * speed * Time.deltaTime;

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

        // ステージに当たったら
        if (other.gameObject.layer == 8)
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
        _isActive = false;
        gameObject.SetActive(false); // オブジェクトを非アクティブ化
        _elapsedTime = 0f;           // 経過時間をリセット
    }

    /// <summary>
    /// 火球を再利用する際の初期化処理。
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