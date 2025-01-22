using Fusion;
using UnityEngine;

public class PlayerAttackTest : NetworkBehaviour
{
    [SerializeField] private float raycastDistance = 10f; // Raycastの距離

    private int health = 100;

    // 初期化処理
    public override void Spawned()
    {
        base.Spawned();

        Debug.Log("a");
        // オブジェクトがサーバーによって管理されているかどうかをチェック
        if (HasStateAuthority)
        {
            // サーバー側でのみ実行される初期化
            health = 100;
            Debug.Log("Player spawned with health: " + health);
        }
        else
        {
            // クライアント側での初期化（必要に応じて）
            Debug.Log("Player object is controlled by another player.");
        }
    }

    private void Update()
    {
        // if (!Object.HasInputAuthority) return; // ローカルプレイヤーのみ処理

        if (Input.GetMouseButtonDown(1)) // 右クリックで攻撃
        {
            RaycastHit hit;

            // プレイヤーの前方方向を取得（playerTransformはプレイヤーのTransform）
            Vector3 forwardDirection = transform.forward;

            // プレイヤーの前方にRayを発射
            Ray ray = new Ray(transform.position, forwardDirection); // プレイヤーの位置から前方に発射

            // Raycastを実行
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                var enemyHealth = hit.collider.GetComponent<EnemyNetworkManager>();
                if (enemyHealth != null)
                {
                    // Raycastでヒットした敵がいた場合
                    Debug.Log($"Hit enemy: {enemyHealth.gameObject.name}");

                    // サーバーに攻撃処理をリクエスト
                    RPC_AttackEnemy(enemyHealth, 20);
                }

                // Raycastが当たった位置を表示
                Debug.Log($"Ray hit at: {hit.point}");
            }

            // Raycastを視覚化（シーンビューでRayを描画）
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 2f); // 2秒間表示
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_AttackEnemy(EnemyNetworkManager enemy, int damage)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(damage); // サーバー側でダメージ処理
        }
    }
}
