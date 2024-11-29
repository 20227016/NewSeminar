
using UnityEngine;
using System.Collections;

/// <summary>
/// EnemyCombatState.cs
/// クラス説明
/// エネミーののけぞりステート
///
/// 作成日: 9/3
/// 作成者: 高橋光栄
/// </summary>
public class EnemyStaggerState : IEnemyState
{
    public void EnterState(OldEnemy enemy)
    {
        Debug.Log("のけぞりステートに移行しました");
    }

    /// <summary>
    /// のけぞらせる(この間,行動停止) 
    /// </summary>
    /// <param name="enemy"></param>
    public void UpdateState(OldEnemy enemy)
    {
        enemy.HandleStagger();
    }

    public void ExitState(OldEnemy enemy)
    {
       
    }
}