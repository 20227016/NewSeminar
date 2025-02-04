
using UnityEngine;
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
    public void ProduceSE(AudioSource audioSource, AudioClip audioClip, float audioSpeed , float audioVolume)
    {

        if(audioSource == null || audioSource == null)
        {

            return;

        }
        audioSource.pitch = audioSpeed;
        audioSource.volume = audioVolume;
        audioSource.PlayOneShot(audioClip);

    }

}
