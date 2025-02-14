
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Fusion;
using UnityEngine.UI;

/// <summary>
/// BaseEnemy.cs
/// クラス説明
///
///
/// 作成日: /
/// 作成者: 
/// </summary>
public abstract class BaseEnemy : NetworkBehaviour,IReceiveDamage
{

    protected Transform TargetTrans { get; set; } = default;

    [SerializeField, Header("無視するレイヤー")]
    protected LayerMask ignoreLayer = default;
    [SerializeField, Header("キャラクターステータス")]
    protected EnemyStatusStruct _enemyStatusStruct = default;
    [SerializeField, Header("無視するレイヤー")]
    protected List<string> _tags = new List<string>();

    [SerializeField,Tooltip("UIのHPバー(Imageコンポーネント)")]
    private Slider _hpBarFill;

    // UniTaskキャンセルトークン
    // private CancellationTokenSource _cancellatToken = default;

    // 探索するときに使用する構造体
    protected BoxCastStruct _boxCastStruct = default;

    protected IEnemyAttack _attack = new EnemyAttack();

    protected IEnemyMove _move = new EnemyMove();

    protected IEnemyAnimation _enemyAnimation = new EnemyAnimation();

    protected float _currentAttackMultiplier = 1;


    [SerializeField] 
    private GameObject _damageTextPrefab; // ダメージテキストのプレハブ
    [SerializeField] 
    private Transform _damageTextSpawnPoint; // ダメージテキストの表示位置

    public override void Spawned()
    {
        base.Spawned();
    }

    private void OnDrawGizmos()
    {
        /// 可視化
        Gizmos.color = Color.red;

        // ゼロベクトルチェック
        if (_boxCastStruct._direction == Vector3.zero)
        {
            return; // 処理を終了
        }

        // 現在の位置を基準にボックスキャストの範囲を描画
        Vector3 start = _boxCastStruct._originPos;

        // 中間地点の描画用に座標を計算
        Vector3 direction = _boxCastStruct._direction.normalized;
        int segmentCount = 10; // 分割数
        float segmentLength = _boxCastStruct._distance / segmentCount;

        // ボックスキャストの全体を描画
        for (int i = 0; i <= segmentCount; i++)
        {
            Vector3 segmentCenter = start + direction * (segmentLength * i);

            // 描画用マトリックス
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(segmentCenter, Quaternion.LookRotation(direction), Vector3.one);
            Gizmos.matrix = rotationMatrix;

            // レイの終端を強調表示
            if (i == segmentCount)
            {   
                Gizmos.color = Color.yellow;
            }

            // セグメントを描画
            Gizmos.DrawWireCube(Vector3.zero, _boxCastStruct._size);
        }
    }

    protected virtual void BasicRaycast()
    {
        SetPostion();
        SetSiz();
        SetDirection();
        SetDistance();
        if (_tags.Count != 0)
        {
            _boxCastStruct._tags = _tags.ToArray();

        }
        // 無視するレイヤー
        _boxCastStruct._layerMask = ~ignoreLayer;
    }

    protected virtual void SetPostion()
    {
        // 自分の目の前から
        // 中心点
        _boxCastStruct._originPos = this.transform.position
            + this.transform.localScale / 2
            - (transform.forward * 5f);
    }


    protected virtual void SetSiz()
    {
        // 半径（直径ではない）
        _boxCastStruct._size = (transform.localScale - Vector3.forward * transform.localScale.z);
        _boxCastStruct._size += Vector3.right * _boxCastStruct._size.x * 2; // X方向を広げる
        _boxCastStruct._size += Vector3.up * _boxCastStruct._size.y * 8;    // Y方向を大きくする
        _boxCastStruct._size -= Vector3.one / 100;
    }


    protected virtual void SetDirection()
    {
        // 方向
        _boxCastStruct._direction = transform.forward;
    }


    protected virtual void SetDistance()
    {
        // 距離
        _boxCastStruct._distance = 5f;
    }

    /// <summary>
    /// ダメージを与える処理
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnTriggerEnter(Collider hitCollider)
    {
        if (!hitCollider.CompareTag("Player"))
        {
            return;
        }

        IReceiveDamage receiveDamage = hitCollider.GetComponent<IReceiveDamage>();
        if (receiveDamage == null)
        {
            return;
        }
        // 攻撃力に攻撃倍率を渡して渡す
        receiveDamage.RPC_ReceiveDamage((int)(_enemyStatusStruct._attackPower * _currentAttackMultiplier));
    }

    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    /// <param name="damegeValue">ダメージ</param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public virtual void RPC_ReceiveDamage(int damegeValue)
    {

        int damage = (int)(damegeValue - (damegeValue * _enemyStatusStruct._defensePercentage));
        // ダメージ処理
        _enemyStatusStruct._hp -= damage;

        // ダメージUIを敵の上に表示する
        Vector3 damagePosition = transform.position + new Vector3(0, 2, 0); 

        // DamageTextにダメージ量と表示する位置を渡す
        FindObjectOfType<DamageText>().ShowDamage(damage, damagePosition);

        // HPUIの更新
        RPC_UpdateHPBar();
        if (_enemyStatusStruct._hp <= 0)
        {
            OnDeath();
        }
        return;
    }

    /// <summary>
    /// 現在のHPとUIの同期処理
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    protected void RPC_UpdateHPBar()
    {
        // HPバーを同期
        if (_hpBarFill != null)
        {
            print("敵のUIと現在のHPを同期させました。UI同期は成功です");
            _hpBarFill.value = (float)_enemyStatusStruct._hp;
        }
    }

    /// <summary>
    /// エネミーの死亡処理(具体的な処理は個別で設定)
    /// </summary>
    protected abstract void OnDeath();


    public virtual void EnemyDespawn()
    {
        if (HasStateAuthority)
        {
            Debug.Log("ですぽーん" + this.name);
            Runner.Despawn(Object);
        }
    }
}