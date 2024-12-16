using Fusion;
using UnityEngine;

public static class GetRunner
{
    
    public static NetworkRunner GetRunnerMethod()
    {

        GameObject nunner = GameObject.FindWithTag("Runner");
        if (nunner == null)
        {

            Debug.LogError("�����i�[�I�u�W�F�N�g������܂���");
            return null;
        }
        nunner.TryGetComponent<NetworkRunner>(out NetworkRunner networkRunner);
        if(networkRunner == null)
        {

            Debug.LogError("�����i�[�R���|�[�l���g������܂���");
            return null;

        }
        return networkRunner;

    }

}
