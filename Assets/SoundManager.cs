using UnityEngine;
using Fusion;

/// <summary>
/// ネットワーク状でサウンドを再生するテストスクリプト
/// 基本的に関数を呼び出し、その関数内でRPCが設定されているEF再生関数を呼び出し、全クライアントに通知する
/// </summary>
public class SoundManager : NetworkBehaviour
{
    [SerializeField]
    private AudioSource _effectAudioSource = default;
    [SerializeField]
    private AudioClip _testClip = default;

    /// <summary>
    /// 効果音再生（サーバー通知）
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void PlayEffectSound(Vector3 position)
    {
        // 音源の位置を設定
        _effectAudioSource.transform.position = position;

        // 効果音を再生
        _effectAudioSource.PlayOneShot(_testClip);
    }

    /// <summary>
    //  効果音を再生
    /// </summary>
    public void TriggerSkillEffect(Vector3 skillPosition)
    {
        if (Object.HasStateAuthority)
        {
            // 全クライアントに通知
            PlayEffectSound(skillPosition);
        }
    }
}
