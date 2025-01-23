using UnityEngine;

/// <summary>
/// MagicBullet.cs
/// 魔法陣を回転させる
/// 作成日: 11/17
/// 作成者: 石井直人 
/// </summary>
public class MagicBullet : MonoBehaviour
{
    [Tooltip("魔弾のダメージ")]
    [SerializeField] private float _damage = 30f;

    private Vector3 _targetScale = new Vector3(5f, 5f, 5f); // 最終的な大きさ
    private float _scaleSpeed = 2f; // 大きくなる速度
    private float _moveSpeed = 5f; // 飛ぶ速度

    private float _currentTimer = 0f; // 現在時間
    private float _displayTime = 5f; // 表示時間

    private bool isMoving = false; // 移動状態フラグ

    private void Update()
    {
        // まだ目標サイズに達していない場合、徐々に大きくする
        if (!isMoving)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, _targetScale, _scaleSpeed * Time.deltaTime);

            // 目標サイズに達したら移動を開始
            if (transform.localScale == _targetScale)
            {
                isMoving = true;
            }
        }
        else
        {
            // 向いている方向に移動
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;

            // 表示時間を計測
            _currentTimer += Time.deltaTime;

            // 一定時間後に非表示
            if (_currentTimer >= _displayTime)
            {
                gameObject.SetActive(false);
            }
        }
    }

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
            Debug.Log($"MagicBullet Hit {other.gameObject.name}, dealt {_damage} damage.");
        }
    }
}
