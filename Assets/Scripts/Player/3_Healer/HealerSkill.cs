using UniRx;
using UnityEngine;

/// <summary>
/// HealerSkill.cs
/// クラス説明
/// ヒーラーのスキル
///
/// 作成日: 9/30
/// 作成者: 山田智哉
/// </summary>
public class HealerSkill : MonoBehaviour, ISkill
{
    // 回復実行間隔定数
    const float HEAL_INTERVAL = 0.1f;

    [SerializeField, Tooltip("スキルの効果範囲(半径)")]
    private float _healAreaRadius = 5.0f;

    [SerializeField, Tooltip("スキルによる回復量( /秒 )")]
    private int _healValue = 10;

    [SerializeField, Tooltip("スキルによるエリア展開までの遅延")]
    private float _skillHitboxDelay = 1.5f;

    // 発動位置
    private Vector3 _actionPosition = default;

    public void Skill(CharacterBase characterBase, float skillDuration)
    {
        Debug.Log("ヒーラーのスキル発動");

        _actionPosition = characterBase.transform.position;

        // 遅延処理
        Observable.Timer(System.TimeSpan.FromSeconds(_skillHitboxDelay))
            .Subscribe(_ =>
            {
                // 発動位置に skillDuration の間、回復エリアを展開
                Observable.Interval(System.TimeSpan.FromSeconds(HEAL_INTERVAL)) // 0.1秒ごとに処理を実行
                    .TakeUntil(Observable.Timer(System.TimeSpan.FromSeconds(skillDuration)))
                    .Subscribe(__ => Heal())
                    .AddTo(this);
            })
            .AddTo(this);
    }

    /// <summary>
    /// 回復
    /// </summary>
    private void Heal()
    {
        // レイを発射
        Collider[] hitColliders = Physics.OverlapSphere(_actionPosition, _healAreaRadius, LayerMask.GetMask("Player"));

        // 1回の呼び出しで与える回復量 (1秒間の回復量を10等分する)
        int healPerTick = _healValue / 10;

        // 当たったプレイヤーを回復
        foreach (Collider collider in hitColliders)
        {
            IReceiveHeal target = collider.GetComponent<IReceiveHeal>();

            if (target != null)
            {
                target.ReceiveHeal(healPerTick);
            }
        }
    }
}