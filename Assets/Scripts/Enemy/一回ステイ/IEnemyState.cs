/// <summary>
/// IEnemyState.cs
/// クラス説明
/// エネミーステートインターフェース
///
/// 作成日: 9/3
/// 作成者: 高橋光栄
/// </summary>
public interface IEnemyState
{
    // ステートに入るときの処理
    void EnterState(OldEnemy enemy);
    // ステートがアクティブなときの処理
    void UpdateState(OldEnemy enemy);
    // ステートから出るときの処理
    void ExitState(OldEnemy enemy);
}