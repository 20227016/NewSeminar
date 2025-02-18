using Cysharp.Threading.Tasks;
using Fusion;
using System;
using UnityEngine;


public class HealingCircle : NetworkBehaviour
{
    public override void Spawned()
    {
        // �w�莞�Ԍ�Ƀf�X�|�[��
        DespawnAfterDelay(10).Forget();
    }

    private async UniTaskVoid DespawnAfterDelay(float delay)
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Play();

        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: this.GetCancellationTokenOnDestroy());

        if (Runner != null && Object != null)
        {
            Runner.Despawn(Object);
        }
    }
}
