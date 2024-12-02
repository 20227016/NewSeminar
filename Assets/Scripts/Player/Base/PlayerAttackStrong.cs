using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerAttackLight.cs
/// クラス説明
/// プレイヤー弱攻撃
/// 
/// 作成日: 9/10
/// 作成者: 山田智哉
/// </summary>
public class PlayerAttackStrong : IAttackStrong
{
    private IComboCounter _comboCounter;

    // 攻撃範囲の半径
    private float attackRadius = 1.5f;

    public PlayerAttackStrong(IComboCounter comboCounter)
    {
        _comboCounter = comboCounter;
    }

    public void AttackStrong(Transform transform, float attackMultiplier)
    {
        Vector3 attackPosition = transform.position + new Vector3(0, 1, 0); // 攻撃の発射地点
        Vector3 attackDirection = transform.forward; // 攻撃の方向

        // 指定した半径内のコライダーを取得
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, attackRadius, LayerMask.GetMask("Enemy"));

        // 敵がヒットした場合の処理
        if (hitColliders.Length <= 0)
        {

            //Debug.Log("攻撃がヒットしませんでした。");
            return;

        }

        foreach (Collider collider in hitColliders)
        {

            IReceiveDamage target = collider.GetComponent<IReceiveDamage>();
            if (target == null)
            {

                return;

            }

            // 敵が攻撃の方向にいるか確認
            Vector3 directionToEnemy = (collider.transform.position - attackPosition).normalized;
            if (Vector3.Dot(directionToEnemy, attackDirection) > 0) // 前方にいるかチェック
            {

                // コンボを追加し、コンボ数を取得
                _comboCounter.AddCombo();
                int currentCombo = _comboCounter.GetCombo();

                // コンボ倍率を計算
                float comboMultiplier = Mathf.Clamp(1 + currentCombo * 0.01f, 1, 2);

                // 与ダメージを計算
                int damage = Mathf.FloorToInt(10 * attackMultiplier * comboMultiplier);

                // 相手にダメージを与える
                target.ReceiveDamage(damage);

                //Debug.Log($"攻撃がヒットしました: {collider.name}, ダメージ: {damage}");
            }

        }
    }
}
