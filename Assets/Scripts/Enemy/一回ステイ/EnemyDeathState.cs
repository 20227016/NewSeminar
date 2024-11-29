
using UnityEngine;
using System.Collections;

/// <summary>
/// EnemyCombatState.cs
/// クラス説明
/// エネミーの死亡ステート
///
/// 作成日: 9/3
/// 作成者: 高橋光栄
/// </summary>
public class EnemyDeathState : IEnemyState
{
    // エネミー死亡
    public void EnterState(OldEnemy enemy)
    {
        Debug.Log("死亡ステートに移行しました");
        enemy.HandleDeath();
    }

    public void UpdateState(OldEnemy enemy)
    {
        // 死亡状態中に何かしたい場合は、ここに記述(処理の内容はEnemyに追加して)
    }

    public void ExitState(OldEnemy enemy)
    {
        // 死亡ステートから出たときに何か処理したいときはここに記述(処理の内容はEnemyに追加して)
    }
}