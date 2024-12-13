using Fusion;
using UnityEngine;

public static class GetRunner
{
    
    public static NetworkRunner GetRunnerMethod()
    {

        GameObject nunner = GameObject.FindWithTag("Runner");
        if (nunner == null)
        {

            Debug.LogError("ランナーオブジェクトがありません");
            return null;
        }
        nunner.TryGetComponent<NetworkRunner>(out NetworkRunner networkRunner);
        if(networkRunner == null)
        {

            Debug.LogError("ランナーコンポーネントがありません");
            return null;

        }
        return networkRunner;

    }

}
