using Fusion;
using UnityEngine;

public class WaveClear : NetworkBehaviour
{

    [Tooltip("�X�e�[�W���킯��d�؂�EF")]
    private GameObject _wavePartition = default;

    [Tooltip("�X�e�[�W���킯��d�؂�EF")]
    private GameObject _wavePartitionEND = default;


    public override void Spawned()
    {
        print("WaveClear��������܂���");

        _wavePartition = GameObject.Find("Gate");
        _wavePartitionEND = GameObject.Find("GateEND");


        //GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        //foreach (GameObject obj in allObjects)
        //{
        //    if (obj.name == "Gate")  // 1��
        //    {
        //        _wavePartition = obj;
        //        print("�擾����: _wavePartition = " + _wavePartition.name);

        //    }
        //    else if (obj.name == "GateEND")  // 2��
        //    {
        //        _wavePartitionEND = obj;
        //        print("�擾����: _wavePartitionEND = " + _wavePartitionEND.name);
        //    }
        //}

        WaveGateOpen();
    }

    private void WaveGateOpen()
    {
        print("WaveGateOpen���Ă΂�܂����BRPC���N�����܂�");

        RPC_WaveGateOpen();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_WaveGateOpen()
    {
        if ((_wavePartition != null) && (_wavePartitionEND != null))
        {
            _wavePartitionEND.SetActive(false);
            _wavePartitionEND.SetActive(true);
            _wavePartition.SetActive(false);
            print("2Wave���N���A���܂����B�Q�[�g�J���I(�l�b�g���[�N)EnemySpawner1�Ŏ��s���܂���");
        }
        else
        {
            print("NULL�ł�");
        }
    }
}
