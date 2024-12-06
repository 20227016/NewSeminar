using UniRx;
using UnityEngine;

/// <summary>
/// HealerSkill.cs
/// クラス説明
///
///
/// 作成日: 9/30
/// 作成者: 山田智哉
/// </summary>
public class HealerSkill : MonoBehaviour, ISkill
{
    [SerializeField, Tooltip("スキルの範囲(半径)")]
    private float _healAreaRadius = 5.0f;

    [SerializeField, Tooltip("スキルによる回復量")]
    private int _healValue = 1;

    // 発動位置
    private Vector3 _actionPosition = default;

    public void Skill(CharacterBase characterBase, float skillDuration)
    {
        Debug.Log("ヒーラーのスキルを発動");

        _actionPosition = characterBase.transform.position;

        // 発動位置にskillDurationの間、回復エリアを展開
        Observable.Interval(System.TimeSpan.FromSeconds(0.1f))
            .TakeUntil(Observable.Timer(System.TimeSpan.FromSeconds(skillDuration)))
            .Subscribe(_ =>
            {
                Heal();
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

        // 当たったプレイヤーを回復
        foreach (Collider collider in hitColliders)
        {
            IReceiveHeal target = collider.GetComponent<IReceiveHeal>();

            if (target != null)
            {
                target.ReceiveHeal(_healValue);
            }
        }
    }
}