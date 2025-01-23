using UnityEngine;
using System.Collections;
using UniRx;

/// <summary>
/// PlayerAttackLight.cs
/// クラス説明
/// プレイヤー強攻撃
/// 
/// 作成日: 9/10
/// 作成者: 山田智哉
/// </summary>
public class PlayerAttackStrong : IAttackStrong
{
    public void AttackStrong(CharacterBase characterBase, float attackPower, float attackMultiplier, float delay, float range)
    {
        // 遅延処理
        Observable.Timer(System.TimeSpan.FromSeconds(delay))
            .Subscribe(_ =>
            {
                Vector3 attackPosition = characterBase.transform.position + new Vector3(0, range, 0); // 攻撃の発射地点
                Vector3 attackDirection = characterBase.transform.forward; // 攻撃の方向

                // 指定した範囲内のコライダーを取得
                Collider[] hitColliders = Physics.OverlapSphere(attackPosition, range, LayerMask.GetMask("Player"));

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
                        int damage = Mathf.FloorToInt(attackPower * attackMultiplier * comboMultiplier);

                        // 相手にダメージを与える
                        target.RPC_ReceiveDamage(damage);

                        // 攻撃がヒットしたことをプレイヤー側に通知
                        characterBase.AttackHit(damage);
                    }
                }
            })
            .AddTo(characterBase);
    }
}
