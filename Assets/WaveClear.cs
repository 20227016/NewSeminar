using Fusion;
using UnityEngine;
using UniRx;
using System.Linq;
using System.Threading.Tasks;

public class WaveClear : NetworkBehaviour
{

    [Tooltip("ステージをわける仕切りEF")]
    private GameObject _wavePartition = default;

    [Tooltip("ステージをわける仕切りEF")]
    private GameObject _wavePartitionEND = default;

    [Tooltip("ボスへ向かうテレポーター")]
    private GameObject _bossTeleporter = default;

    /// <summary>
    /// 敵管理クラス
    /// </summary>
    private EnemySpawner _enemySpawner = default;

    private NetworkRunner runner = default;

    public override void Spawned()
    {
        print("WaveClear生成されました");

        _wavePartition = GameObject.Find("Gate");
        _wavePartitionEND = GameObject.Find("GateEND");
        _bossTeleporter = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "BosteReporter");
        _enemySpawner = FindObjectOfType<EnemySpawner>();

        _enemySpawner.OnAllEnemiesDefeatedObservable.Subscribe(_ =>
        {
            // エネミー全滅時の処理を記述
            Debug.Log("他のスクリプトでエネミー全滅イベントを受け取りました！");
            HandleAllEnemiesDefeated();
        }).AddTo(this);

        WaveGateOpen();
    }

    private void WaveGateOpen()
    {
        print("WaveGateOpenが呼ばれました。RPCを起動します");

        RPC_WaveGateOpen1();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_WaveGateOpen1()
    {
        if ((_wavePartition != null) && (_wavePartitionEND != null))
        {
            _wavePartitionEND.SetActive(false);
            _wavePartitionEND.SetActive(true);
            WaveGateOpen2();
        }
        else
        {
            print("NULLです");
        }
    }

    private async void WaveGateOpen2()
    {
        await Task.Delay(1000);
        RPC_WaveGateOpen3();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_WaveGateOpen3()
    {
        _wavePartition.SetActive(false);
    }

    private void HandleAllEnemiesDefeated()
    {
        RPC_HandleAllEnemiesDefeated();
    }

    /// <summary>
    /// 敵が全滅したら呼ばれる処理
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_HandleAllEnemiesDefeated()
    {
        print("敵が全滅しました。テレポーター出現処理を実行させます");
        _enemySpawner.BossStart(runner);
    }
}
