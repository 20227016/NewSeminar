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
public class PlayerAttackLight : IAttackLight
{
    // 攻撃範囲の半径
    private float _attackRadius = 1.5f;

    public void AttackLight(CharacterBase characterBase,  float attackPower, float attackMultiplier)
    {
        Vector3 attackPosition = characterBase.transform.position + new Vector3(0, 1, 0); // 攻撃の発射地点
        Vector3 attackDirection = characterBase.transform.forward; // 攻撃の方向

        // 指定した半径内のコライダーを取得
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, _attackRadius, LayerMask.GetMask("Enemy"));

        if (hitColliders.Length <= 0) return;

        foreach (Collider collider in hitColliders)
        {
            Debug.Log("攻撃がヒット" + collider.name);
            
            IReceiveDamage target = collider.GetComponent<IReceiveDamage>();
            if (target == null) return;

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
                int damage = Mathf.FloorToInt(attackPower * attackMultiplier * comboMultiplier);

                // 相手にダメージを与える
                target.ReceiveDamage(damage);

                // 攻撃がヒットしたことをプレイヤー側に返す
                characterBase.AttackHit();

            }

        }
    }
}
