using UnityEngine;
using System.Collections.Generic;
using Fusion;

/// <summary>
/// MagicBullet.cs
/// 魔法陣を回転させる
/// 作成日: 11/17
/// 作成者: 石井直人 
/// </summary>
public class MagicBullet : BaseEnemy
{
    private Vector3 _targetScale = new Vector3(5f, 5f, 5f); // 最終的な大きさ
    private float _scaleSpeed = 2f; // 大きくなる速度
    private float _moveSpeed = 5f; // 飛ぶ速度

    private float _currentTimer = 0f; // 現在時間
    private float _displayTime = 5f; // 表示時間

    private bool isMoving = false; // 移動状態フラグ

    [SerializeField, Tooltip("探索範囲(前方距離)")]
    protected float _searchRange = default;

    //AudioSource型の変数を宣言
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip型の変数を宣言
    [SerializeField] private AudioClip _magicBulletSE = default;

    /// <summary>
    /// 効果音発生
    /// </summary>
    private void Start()
    {
        _audioSource.PlayOneShot(_magicBulletSE);
    }

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
                Destroy(gameObject);
            }
        }
        SetPostion();

    }

    /// <summary>
    /// キャストの位置
    /// </summary>
    protected override void SetPostion()
    {
        // 自分の目の前から
        // 中心点
        _boxCastStruct._originPos = this.transform.position;
    }

    /// <summary>
    /// キャストの半径
    /// </summary>
    protected override void SetSiz()
    {
        // 半径（直径ではない）
        _boxCastStruct._size = Vector3.one * _searchRange;
    }

    /// <summary>
    /// レイキャストの距離(探索範囲)
    /// </summary>
    protected override void SetDistance()
    {
        base.SetDistance();
        _boxCastStruct._distance = 0;
    }

    /// <summary>
    /// プレイヤーを探す
    /// </summary>
    private void RPC_PlayerSearch()
    {
        if (TargetTrans != null) return;

        int layerMask = (1 << 6) | (1 << 8); // レイヤーマスク（レイヤー6と8）

        Collider[] hits = Physics.OverlapSphere(_boxCastStruct._originPos, _searchRange, layerMask);

        // ボックスキャストの実行
        if (hits.Length > 0)
        {
            List<Collider> playerColliders = default;
            foreach (Collider hit in hits)
            {
                if (hit.gameObject.layer == 6)
                {
                    playerColliders.Add(hit);
                }
            }
            // プレイヤー（レイヤー6）の場合の処理
            if (playerColliders != null)
            {
                //TargetTrans = playerCollider.gameObject.transform;
                //_playerLastKnownPosition = TargetTrans.position; // プレイヤーの位置を記録
                //_movementState = EnemyMovementState.RUNNING;
            }
            else
            {
                TargetTrans = null; // プレイヤー以外ならターゲットを解除
            }
        }
        else
        {
            // ヒットしなかった場合
            TargetTrans = null;
        }
    }

    /// <summary>
    /// HPが0以下になったら呼ばれる処理(Base参照)
    /// </summary>
    protected override void OnDeath()
    {

    }

    public override void RPC_ReceiveDamage(int damegeValue)
    {

    }
}
