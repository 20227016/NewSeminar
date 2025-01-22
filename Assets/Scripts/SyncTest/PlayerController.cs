using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public GameObject playerPrefab; // プレイヤーのプレハブ
    private NetworkRunner _networkRunner; // NetworkRunnerの参照

    private void Start()
    {
        // NetworkRunnerの参照を取得
        _networkRunner = FindObjectOfType<NetworkRunner>();

        // サーバー側でのみ実行
        if (Object.HasStateAuthority) // サーバー権限がある場合
        {
            // サーバーでオブジェクトをスポーンする
            SpawnPlayerObject();
        }
    }

    private void SpawnPlayerObject()
    {
        // NetworkRunnerが存在していることを確認
        if (_networkRunner != null)
        {
            // プレイヤーオブジェクトをインスタンス化
            GameObject playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

            // サーバー側でスポーン
            _networkRunner.Spawn(playerObject, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("NetworkRunner is not found in the scene.");
        }
    }
}
