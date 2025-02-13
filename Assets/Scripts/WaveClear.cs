using Fusion;
using UnityEngine;
using UniRx;
using System.Linq;
using System.Threading.Tasks;
using System;

public class WaveClear : NetworkBehaviour
{

    [Tooltip("�X�e�[�W���킯��d�؂�EF")]
    private GameObject _wavePartition = default;

    [Tooltip("�X�e�[�W���킯��d�؂�EF")]
    private GameObject _wavePartitionEND = default;

    /// <summary>
    /// �G�Ǘ��N���X
    /// </summary>
    private EnemySpawner _enemySpawner = default;

    public override void Spawned()
    {
        print("WaveClear��������܂���");

        _wavePartition = GameObject.Find("Gate");
        _wavePartitionEND = GameObject.Find("GateEND");
        _enemySpawner = FindObjectOfType<EnemySpawner>();

        WaveGateOpen();
    }

    private void WaveGateOpen()
    {
        print("WaveGateOpen���Ă΂�܂����BRPC���N�����܂�");

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
            print("NULL�ł�");
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
