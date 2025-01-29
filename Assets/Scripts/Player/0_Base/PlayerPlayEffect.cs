using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class PlayerPlayEffect : IPlayEffect
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayEffect(NetworkObject particleSystem, Vector3 playPosition)
    {
        // エフェクトの再生位置を設定
        particleSystem.transform.position = playPosition;

        particleSystem.gameObject.SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public async void RPC_LoopEffect(NetworkObject particleObject, Vector3 playPosition, float effectDuration)
    {
        // エフェクトの再生位置を設定
        particleObject.transform.position = playPosition;

        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

        // 再生時間を取得
        float particleDuration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;

        // ループ回数を計算
        int loopCount = Mathf.FloorToInt(effectDuration / particleDuration);

        // ループを無効にして最初の再生
        particleObject.gameObject.SetActive(true);

        // 非同期でループ処理を実行
        await RPC_PlayEffectLoop(particleSystem, loopCount, particleDuration);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private async UniTask RPC_PlayEffectLoop(ParticleSystem particleSystem, int loopCount, float particleDuration)
    {
        for (int i = 1; i < loopCount; i++) 
        {
            await UniTask.Delay((int)(particleDuration * 1000));

            particleSystem.Play(); // 再生
        }

        // 最後の再生を待機して非表示
        await UniTask.Delay((int)(particleDuration * 1000));

        particleSystem.gameObject.SetActive(false);
    }
}