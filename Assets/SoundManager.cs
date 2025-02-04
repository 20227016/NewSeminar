
using UnityEngine;
using Cysharp.Threading.Tasks;
using Fusion;

/// <summary>
/// ネットワーク状でサウンドを再生するテストスクリプト
/// 基本的に関数を呼び出し、その関数内でRPCが設定されているEF再生関数を呼び出し、全クライアントに通知する
/// </summary>
public class SoundManager : ISound
{

    /// <summary>
    /// 効果音を出力
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="audioClip"></param>
    /// <param name="audioSpeed"></param>
    /// <param name="audioVolume"></param>
    /// <param name="delay"></param>
    public async void ProduceSE(AudioSource audioSource, AudioClip audioClip, float audioSpeed , float audioVolume , float delay)
    {

        if(audioSource == null || audioSource == null)
        {

            return;

        }
        await UniTask.Delay((int)(delay * 1000f));
        audioSource.pitch = audioSpeed;
        audioSource.volume = audioVolume;
        audioSource.PlayOneShot(audioClip);

    }

}
