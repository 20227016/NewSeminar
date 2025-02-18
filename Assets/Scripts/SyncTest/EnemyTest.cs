using Fusion;
using UnityEngine;

public class EnemyTest : BaseEnemy
{
    [Networked]
    public int Health { get; private set; } // 同期される体力
    private EnemyNetworkManager _networkManager;

    //[SerializeField]
    //private int maxHealth = 100; // 最大体力

    [SerializeField]
    private GameObject deathEffectPrefab; // 死亡エフェクト

    private void Awake()
    {
        // ネットワーク管理クラスを取得
        _networkManager = GetComponent<EnemyNetworkManager>();
    }

    /*
    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void OnNetworkSpawn()
    { 
        if (Object.HasStateAuthority) // サーバー権限がある場合
        {
            Health = maxHealth; // サーバー側で初期化
        }    
    }
    */

    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    /// <param name="damage">受けたダメージ量</param>
    public void TakeDamage(int damage)
    {
        // if (!Object.HasStateAuthority) return; // サーバーのみが処理

        if (Health <= 0) return; // 既に死亡している場合は無視

        Health -= damage;
        Health = Mathf.Max(Health, 0); // 最小値を0に制限

        Debug.Log($"{gameObject.name} took {damage} damage, remaining health: {Health}");

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
        Debug.Log($"{gameObject.name} has died.");

        if (deathEffectPrefab != null)
        {
            // 死亡エフェクトを生成
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Runner.Despawn(Object); // ネットワークオブジェクトを削除
    }

    /// <summary>
    /// クライアント向けの同期用UIメソッド
    /// </summary>
    public int GetHealth()
    {
        return Health;
    }

    /// <summary>
    /// HPが0以下になったら呼ばれる処理(Base参照)
    /// </summary>
    protected override void OnDeath()
    {
    
    }
}
