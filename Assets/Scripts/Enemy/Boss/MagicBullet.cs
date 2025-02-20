using UnityEngine;
using System.Collections.Generic;
using Fusion;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UniRx;


/// <summary>
/// MagicBullet.cs
/// 魔法陣を回転させる
/// 作成日: 11/17
/// 作成者: 石井直人 
/// </summary>
public class MagicBullet : BaseEnemy
{

    private List<Collider> _playerColliders = new List<Collider>();

    private Vector3 _targetScale = new Vector3(5f, 5f, 5f); // 最終的な大きさ
    private float _scaleSpeed = 2f; // 大きくなる速度
    private float _moveSpeed = 5f; // 飛ぶ速度

    private float _currentTimer = 0f; // 現在時間
    private float _displayTime = 5f; // 表示時間

    private bool isMoving = false; // 移動状態フラグ

    [SerializeField, Header("探索範囲")]
    protected float _searchRange = default;

    [SerializeField, Header("吸い込みの速さ")]
    protected float _suctionSpeed = default;

    [SerializeField, Header("吸い込みの間隔")]
    protected float _delayTime = 0.1f;

    //AudioSource型の変数を宣言
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip型の変数を宣言
    [SerializeField] private AudioClip _magicBulletSE = default;

    private Vector3 _myPos = default;

    private float _delayTimer = default;

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
        PlayerSearch();
        if(_playerColliders.Count <= 0)
        {

            return;

        }
        if(this.gameObject != null)
        {

            _myPos = this.gameObject.transform.position;

        }
       
        if(_delayTime > _delayTimer)
        {

            _delayTimer += Time.deltaTime;
            return;

        }
        _delayTimer = 0f;
        foreach (Collider collider in _playerColliders)
        {

            Vector3 direction = _myPos - collider.transform.position;

            direction.y = 0;
            collider.transform.position += direction * _suctionSpeed * Time.deltaTime;
        }

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
    private void PlayerSearch()
    {
        if (TargetTrans != null) return;

        int layerMask = (1 << 6) | (1 << 8); // レイヤーマスク（レイヤー6と8）

        Collider[] hits = Physics.OverlapSphere(_boxCastStruct._originPos, _searchRange, layerMask);

        // ボックスキャストの実行
        if (hits.Length > 0)
        {

            foreach (Collider hit in hits)
            {
                if (hit.gameObject.layer == 6)
                {

                    _playerColliders.Add(hit);
                }
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

    public override void ReceiveDamage(int damegeValue)
    {

    }
}
