
using UnityEngine;
using System.Collections;

/// <summary>
/// Enemy.cs
/// クラス説明
/// エネミーのステートを管理するためのスクリプト
///
/// 作成日: 9/3
/// 作成者: 高橋光栄
/// </summary>
public class OldEnemy : MonoBehaviour
{

    private IEnemyState currentState;
    public IEnemyState CurrentState { get; private set; }

    public EnemyCombatState EnemyCombatState = new EnemyCombatState();
    public EnemyStaggerState EnemyStaggerState = new EnemyStaggerState();
    public EnemyDeathState EnemyDeathState = new EnemyDeathState();

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {

        // 初期状態を戦闘ステートに設定
        currentState = EnemyCombatState;
        currentState.EnterState(this);
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        currentState.UpdateState(this);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {  　     
        // 常に実行させる
  　    currentState.UpdateState(this);
    }

    public void TransitionToState(IEnemyState newState)
    {

        if (currentState != newState)
        {
            // 現在のステートから離脱
            currentState.ExitState(this);

            // 新しいステートへの移行
            currentState = newState;
            currentState.EnterState(this);
        }
        else
        {
            print("切り替え先のステートが同じです。適切に切り替えてください");
        }

        // 次の行いたいステートへ移行したい場合は
        // enemy.TransitionToState(enemy.ステート名);
        // でステート切り替えできます。

    }

    //--------------------------------------------------------------------エネミーの各行動

    public void HandleCombat()
    {
        Debug.Log("戦闘処理を書く");
        // ここはストラテジーパターンで記述予定だけど、ゆーちゃんの設計待ち
        TransitionToState(EnemyStaggerState);
    }

    public void HandleStagger()
    {
        Debug.Log("この程度でのけぞらないといけない俺の気持ち考えたことある？");
        // 多分のけぞりのアニメーションとかの処理書くんじゃね？知ってるけど
        TransitionToState(EnemyDeathState);
    }

    public void HandleDeath()
    {
        Debug.Log("ちんだ⑨");
        // 敵はオブジェクトプールとかで管理すると思うから、配列に敵のオブジェクトを返す
        // メソッドを書くことになるのかな？ここの適正はやまやま相談
    }
}