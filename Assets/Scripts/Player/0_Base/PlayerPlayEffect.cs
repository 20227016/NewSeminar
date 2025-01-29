using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class PlayerPlayEffect : IPlayEffect
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayEffect(NetworkObject particleSystem, Vector3 playPosition)
    {
        // �G�t�F�N�g�̍Đ��ʒu��ݒ�
        particleSystem.transform.position = playPosition;

        particleSystem.gameObject.SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public async void RPC_LoopEffect(NetworkObject particleObject, Vector3 playPosition, float effectDuration)
    {
        // �G�t�F�N�g�̍Đ��ʒu��ݒ�
        particleObject.transform.position = playPosition;

        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

        // �Đ����Ԃ��擾
        float particleDuration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;

        // ���[�v�񐔂��v�Z
        int loopCount = Mathf.FloorToInt(effectDuration / particleDuration);

        // ���[�v�𖳌��ɂ��čŏ��̍Đ�
        particleObject.gameObject.SetActive(true);

        // �񓯊��Ń��[�v���������s
        await RPC_PlayEffectLoop(particleSystem, loopCount, particleDuration);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private async UniTask RPC_PlayEffectLoop(ParticleSystem particleSystem, int loopCount, float particleDuration)
    {
        for (int i = 1; i < loopCount; i++) 
        {
            await UniTask.Delay((int)(particleDuration * 1000));

            particleSystem.Play(); // �Đ�
        }

        // �Ō�̍Đ���ҋ@���Ĕ�\��
        await UniTask.Delay((int)(particleDuration * 1000));

        particleSystem.gameObject.SetActive(false);
    }
}