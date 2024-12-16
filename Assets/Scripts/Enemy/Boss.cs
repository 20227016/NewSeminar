
using UnityEngine;
using System.Collections;

/// <summary>
/// Boss.cs
/// ボスの行動ロジックを管理するクラス。
/// 攻撃など、さまざまな状態に応じた動きを制御する。
/// 作成日: 12/13
/// 作成者: 石井直人, 北構天哉

/// </summary>
public class Boss : BaseEnemy
{
    /// <summary>
    /// ボスの状態を表す列挙型
    /// </summary>
    private enum BossMovementState
    {
        IDLE,        // 待機
        DOWNED,      // ダウン
        DIE,         // 死亡
    }

    /// <summary>
    /// ボスの攻撃状態を表す列挙型
    /// </summary>
    private enum BossAttackState
    {
        IDLE,        // 待機
        WING,        // つばさでうつ
        MAGIC,       // シャドーボール
        BEAM,        // はかいこうせん
        SUMMON       // 雑魚敵召喚
    }

    [SerializeField]
    private BossMovementState _movementState = BossMovementState.IDLE;

    [SerializeField]
    private BossAttackState _attacktState = BossAttackState.IDLE;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {

    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    protected void Update()
    {
        switch (_movementState)
        {
            // 基本状態
            case BossMovementState.IDLE:

                break;

            // ダウン(ブレイク状態)
            case BossMovementState.DOWNED:

                return;

            // 死亡
            case BossMovementState.DIE:

                return;
        }

        switch (_attacktState)
        {
            // 攻撃待機
            case BossAttackState.IDLE:

                break;

            // つばさでうつ
            case BossAttackState.WING:

                break;

            // シャドーボール
            case BossAttackState.MAGIC:

                break;

            // はかいこうせん
            case BossAttackState.BEAM:

                break;

            // 雑魚敵召喚
            case BossAttackState.SUMMON:

                break;
        }
    }

    /// <summary>
    /// 基本状態
    /// </summary>
    private void BossIdle()
    {

    }

    /// <summary>
    /// ダウン
    /// </summary>
    private void BossDowned()
    {

    }

    /// <summary>
    /// 死ぬ
    /// </summary>
    private void BossDie()
    {

    }

    // <summary>
    /// 攻撃待機
    /// </summary>
    private void AttackIdle()
    {

    }

    // <summary>
    /// つばさでうつ
    /// </summary>
    private void WingAttack()
    {

    }

    // <summary>
    /// シャドーボール
    /// </summary>
    private void MagicAttack()
    {

    }

    // <summary>
    /// はかいこうせん
    /// </summary>
    private void BeamAttack()
    {

    }

    // <summary>
    /// 雑魚敵召喚
    /// </summary>
    private void Summon()
    {

    }
}