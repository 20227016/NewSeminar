using UnityEngine;

[System.Serializable]
public struct CharacterSoundStruct
{

    [SerializeField, Header("自身のサウンドソース")]
    public AudioSource _audioSource;
    [Space(20)]
    [SerializeField, Header("１回目の攻撃時の音")]
    public AudioClip _attack_1;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_1;
    [SerializeField, Header("音量"), Range(0f, 1f)]
    public float _audioVolume_1;
    [SerializeField, Header("遅れ"), Min(0f)]
    public float _delay_1;
    [Space(20)]
    [SerializeField, Header("２回目の攻撃時の音")]
    public AudioClip _attack_2;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_2;
    [SerializeField, Header("音量"), Range(0f, 1f)]
    public float _audioVolume_2;
    [SerializeField, Header("遅れ"), Min(0f)]
    public float _delay_2;
    [Space(20)]
    [SerializeField, Header("３回目の攻撃時の音")]
    public AudioClip _attack_3;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_3;
    [SerializeField, Header("音量"), Range(0f, 1f)]
    public float _audioVolume_3;
    [SerializeField, Header("遅れ"), Min(0f)]
    public float _delay_3;
    [Space(20)]
    [SerializeField, Header("強めの攻撃時の音")]
    public AudioClip _attack_Strong;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_Strong;
    [SerializeField, Header("音量"), Range(0f, 1f)]
    public float _audioVolume_Strong;
    [SerializeField, Header("遅れ"), Min(0f)]
    public float _delay_Strong;
    [Space(20)]
    [SerializeField, Header("必殺技の音")]
    public AudioClip _attack_Special;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_Special;
    [SerializeField, Header("音量"), Range(0f, 1f)]
    public float _audioVolume_Special;
    [SerializeField, Header("遅れ"), Min(0f)]
    public float _delay_Special;
    [Space(20)]
    [SerializeField, Header("被弾音")]
    public AudioClip _getHit;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_GetHit;
    [SerializeField, Header("音量"), Range(0f, 1f)]
    public float _audioVolume_GetHit;
    [SerializeField, Header("遅れ"), Min(0f)]
    public float _delay_GetHit;
    [Space(20)]
    [SerializeField, Header("回避時の音\n※タンクは盾を構える音")]
    public AudioClip _dodge;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_Dodge;
    [SerializeField, Header("音量"), Range(0f, 1f)]
    public float _audioVolume_Dodge;
    [SerializeField, Header("遅れ"), Min(0f)]
    public float _delay_Dodge;

}

