
using UnityEngine;
using System.Collections;

/// <summary>
/// EnemyAnimation.cs
/// クラス説明
///
///
/// 作成日: /
/// 作成者: 
/// </summary>
public class EnemyAnimation2 : IEnemyAnimation
{


    /// <summary>
    /// 移動
    /// 方向の引数は
    /// ０：待機
    /// １：前進
    /// ２：後退
    /// ３：右歩き
    /// ４：左歩き
    /// </summary>
    /// <param name="_animator">呼び出したオブジェクトのアニメーター</param>
    /// <param name="dirctionID">移動方向のID</param>
    public void Movement(Animator _animator, int dirctionID)
    {

        switch (dirctionID)
        {


            case 0:

                _animator.Play("Idle",0);

                break;
            // 前進
            case 1:

                _animator.Play("WalkFWD",0);

                break;
            // 後退
            case 2:

                _animator.Play("WalkBWD", 0);

                break;
            // 右歩き
            case 3:

                _animator.Play("WalkRight", 0);

                break;
            // 左歩き
            case 4:

                _animator.Play("WalkLeft", 0);

                break;

            // ダウン
            case 5:

                _animator.Play("Down",2);

                break;

            // 死亡
            case 6:

                _animator.Play("Die",2);

                break;

        }


    }

    /// <summary>
    /// 攻撃アニメーション
    /// </summary>
    /// <param name="_animator">呼び出したオブジェクトのアニメーター</param>
    /// <param name="_attackID">攻撃ナンバー</param>
    public void Attack(Animator _animator, int _attackID)
    {

        _animator.Play("Attack" + _attackID,1);

    }

}