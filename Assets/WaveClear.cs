using Fusion;
using UnityEngine;
using UniRx;
using System.Linq;
using System.Threading.Tasks;
using System;

public class WaveClear : NetworkBehaviour
{

    [Tooltip("ステージをわける仕切りEF")]
    private GameObject _wavePartition = default;

    [Tooltip("ステージをわける仕切りEF")]
    private GameObject _wavePartitionEND = default;

    /// <summary>
    /// 敵管理クラス
    /// </summary>
    private EnemySpawner _enemySpawner = default;

    public override void Spawned()
    {
        print("WaveClear生成されました");

        _wavePartition = GameObject.Find("Gate");
        _wavePartitionEND = GameObject.Find("GateEND");
        _enemySpawner = FindObjectOfType<EnemySpawner>();

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
}
