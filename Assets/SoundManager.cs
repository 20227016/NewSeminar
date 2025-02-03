using UnityEngine;
using Fusion;

/// <summary>
/// �l�b�g���[�N��ŃT�E���h���Đ�����e�X�g�X�N���v�g
/// ��{�I�Ɋ֐����Ăяo���A���̊֐�����RPC���ݒ肳��Ă���EF�Đ��֐����Ăяo���A�S�N���C�A���g�ɒʒm����
/// </summary>
public class SoundManager : NetworkBehaviour
{
    [SerializeField]
    private AudioSource _effectAudioSource = default;
    [SerializeField]
    private AudioClip _testClip = default;

    /// <summary>
    /// ���ʉ��Đ��i�T�[�o�[�ʒm�j
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void PlayEffectSound(Vector3 position)
    {
        // �����̈ʒu��ݒ�
        _effectAudioSource.transform.position = position;

        // ���ʉ����Đ�
        _effectAudioSource.PlayOneShot(_testClip);
    }

    /// <summary>
    //  ���ʉ����Đ�
    /// </summary>
    public void TriggerSkillEffect(Vector3 skillPosition)
    {
        if (Object.HasStateAuthority)
        {
            // �S�N���C�A���g�ɒʒm
            PlayEffectSound(skillPosition);
        }
    }
}
