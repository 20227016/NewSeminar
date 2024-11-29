
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

/// <summary>
/// BaseEnemy.cs
/// クラス説明
///
///
/// 作成日: /
/// 作成者: 
/// </summary>
public abstract class BaseEnemy : MonoBehaviour,IReceiveDamage
{

    [SerializeField, Header("無視するレイヤー")]
    protected LayerMask ignoreLayer = default;
    [SerializeField, Header("キャラクターステータス")]
    protected EnemyStatusStruct _enemyStatusStruct = default;
    [SerializeField, Header("無視するレイヤー")]
    protected List<string> _tags = new List<string>();

    // UniTaskキャンセルトークン
    private CancellationTokenSource _cancellatToken = default;

    // 探索するときに使用する構造体
    protected BoxCastStruct _boxCastStruct = default;

    protected IEnemyAttack _attack = new EnemyAttack();

    protected IEnemyMove _move = new EnemyMove();

    protected IEnemyAnimation _enemyAnimation = new EnemyAnimation();

    protected float _currentAttackMultiplier = 1;

    private void OnDrawGizmos()
    {

        /// 可視化
        Gizmos.color = Color.red;

        // 現在の位置を基準にボックスキャストの範囲を描画
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(_boxCastStruct._originPos, Quaternion.LookRotation(_boxCastStruct._direction), Vector3.one);
        Gizmos.matrix = rotationMatrix;

        // BoxCastの可視化
        Gizmos.DrawWireCube(Vector3.zero, _boxCastStruct._size);

        // レイの先の地点を描画
        Gizmos.DrawWireCube(_boxCastStruct._direction * _boxCastStruct._distance, _boxCastStruct._size);

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
        _boxCastStruct._originPos = this.transform.position + this.transform.localScale / 2;

    }

    protected virtual void SetSiz()
    {

        // 半径（直径ではない）
        _boxCastStruct._size = (transform.localScale - Vector3.forward * transform.localScale.z);
        _boxCastStruct._size += Vector3.right * _boxCastStruct._size.x * 2;
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
        _boxCastStruct._distance = 5f
;
    }

    /// <summary>
    /// ダメージを与える処理
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnTriggerEnter(Collider hitCollider)
    {

        IReceiveDamage receiveDamage = hitCollider.GetComponent<IReceiveDamage>();
        if (receiveDamage == null)
        {

            return;

        }
        // 攻撃力に攻撃倍率を渡して渡す
        receiveDamage.ReceiveDamage((int)(_enemyStatusStruct._attackPower * _currentAttackMultiplier));
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damegeValue">ダメージ</param>
    public void ReceiveDamage(int damegeValue)
    {

        _enemyStatusStruct._hp -= damegeValue - _enemyStatusStruct._diffencePower;

    }

}