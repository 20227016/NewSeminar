using UniRx;
using UnityEngine;

/// <summary>
/// NormalSkill.cs
/// クラス説明
/// ノーマルキャラのスキル
///
/// 作成日: 9/25
/// 作成者: 山田智哉
/// </summary>
public class NormalSkill : MonoBehaviour, ISkill
{
    [SerializeField, Tooltip("スキルによる攻撃のダメージ倍率( * 攻撃力)")]
    private float _skillAttackMultiplier = 3.0f;

    [SerializeField, Tooltip("スキルによる攻撃の当たり判定発生遅延")]
    private float _skillAttacHitboxkDelay = 0.2f;

    [SerializeField, Tooltip("スキルによる攻撃の当たり判定範囲")]
    private float _skillAttackHitboxRange = 2.0f;

    public void Skill(CharacterBase characterBase, float skillTime)
    {
        Debug.Log("ノーマルのスキル発動");
        // 遅延処理
        Observable.Timer(System.TimeSpan.FromSeconds(_skillAttacHitboxkDelay))
            .Subscribe(_ =>
            {
                Vector3 attackPosition = characterBase.transform.position + new Vector3(0, _skillAttackHitboxRange, 0); // 攻撃の発射地点
                Vector3 attackDirection = characterBase.transform.forward; // 攻撃の方向

                // 指定した範囲内のコライダーを取得
                Collider[] hitColliders = Physics.OverlapSphere(attackPosition, _skillAttackHitboxRange, LayerMask.GetMask("Enemy"));

                if (hitColliders.Length <= 0) return;

                foreach (Collider collider in hitColliders)
                {
                    IReceiveDamage target = collider.GetComponent<IReceiveDamage>();
                    if (target == null) continue;

                    // 敵が攻撃の方向にいるか確認
                    Vector3 directionToEnemy = (collider.transform.position - attackPosition).normalized;

                    if (Vector3.Dot(directionToEnemy, attackDirection) > 0) // 前方にいるかチェック
                    {
                        ComboCounter comboCounter = ComboCounter.Instance;

                        // コンボ数を加算
                        comboCounter.AddCombo();

                        // 現在のコンボ倍率を取得
                        float comboMultiplier = comboCounter.GetComboMultiplier();

                        // 与ダメージを計算
                        int damage = Mathf.FloorToInt(characterBase._characterStatusStruct._attackPower * _skillAttackMultiplier * comboMultiplier);

                        // 相手にダメージを与える
                        target.ReceiveDamage(damage);

                        // 攻撃がヒットしたことをプレイヤー側に通知
                        characterBase.AttackHit(damage);
                    }
                }
            })
            .AddTo(characterBase);
    }
}