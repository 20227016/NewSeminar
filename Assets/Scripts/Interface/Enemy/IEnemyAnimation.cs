using UnityEngine;

public interface IEnemyAnimation
{

    /// <summary>
    /// 移動
    /// 方向の引数は
    /// １：全身
    /// ２：後退
    /// ３：右歩き
    /// ４：左歩き
    /// </summary>
    /// <param name="_animator">呼び出したオブジェクトのアニメーター</param>
    /// <param name="dirctionID">移動方向のID</param>
    public void Movement(Animator _animator, int dirctionID);

    /// <summary>
    /// 攻撃アニメーション
    /// </summary>
    /// <param name="_animator">呼び出したオブジェクトのアニメーター</param>
    /// <param name="_attackID">攻撃ナンバー</param>
    public void Attack(Animator _animator, int _attackID);

}