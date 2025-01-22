using Fusion;
using UnityEngine;

public class EnemyNetworkManager : NetworkBehaviour
{
    [Networked] // クライアント間で同期される変数
    public int Health { get; private set; } // 体力を同期

    [SerializeField]
    private int maxHealth = 100;

    /// <summary>
    /// 初期化
    /// </summary>
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Health = maxHealth; // サーバー側で初期化
        }
    }

    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    /// <param name="damage">受けるダメージ量</param>
    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority) return; // サーバーのみが処理

        if (Health <= 0) return;

        Health -= damage;
        Health = Mathf.Max(Health, 0); // 最小値を0に制限

        Debug.Log($"Enemy {gameObject.name} took {damage} damage, remaining health: {Health}");

        if (Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 敵が死亡したときの処理
    /// </summary>
    private void Die()
    {
        Debug.Log($"Enemy {gameObject.name} has died.");
        Runner.Despawn(Object); // サーバー側で削除
    }
}
