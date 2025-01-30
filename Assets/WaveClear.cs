using Fusion;
using UnityEngine;

public class WaveClear : NetworkBehaviour
{

    [Tooltip("ステージをわける仕切りEF")]
    private GameObject _wavePartition = default;

    [Tooltip("ステージをわける仕切りEF")]
    private GameObject _wavePartitionEND = default;


    public override void Spawned()
    {
        print("WaveClear生成されました");

        _wavePartition = GameObject.Find("Gate");
        _wavePartitionEND = GameObject.Find("GateEND");


        //GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        //foreach (GameObject obj in allObjects)
        //{
        //    if (obj.name == "Gate")  // 1つ目
        //    {
        //        _wavePartition = obj;
        //        print("取得成功: _wavePartition = " + _wavePartition.name);

        //    }
        //    else if (obj.name == "GateEND")  // 2つ目
        //    {
        //        _wavePartitionEND = obj;
        //        print("取得成功: _wavePartitionEND = " + _wavePartitionEND.name);
        //    }
        //}

        WaveGateOpen();
    }

    private void WaveGateOpen()
    {
        print("WaveGateOpenが呼ばれました。RPCを起動します");

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
            print("2Waveをクリアしました。ゲート開放！(ネットワーク)EnemySpawner1で実行しました");
        }
        else
        {
            print("NULLです");
        }
    }
}
