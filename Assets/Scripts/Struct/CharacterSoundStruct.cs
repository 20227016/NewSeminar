using UnityEngine;

[System.Serializable]
public struct CharacterSoundStruct
{
    
     [SerializeField, Header("自身のサウンドソース")]
    public AudioSource _audioSource;

    [SerializeField,Header("１回目の攻撃時の音")]
    public AudioClip _attack_1;
    [SerializeField, Header("再生速度"),Range(0f, 2f)]
    public float _playBackSpeed_1;    
    [SerializeField, Header("音量"), Range(0f, 2f)]
    public float _audioVolume_1;

    [SerializeField, Header("２回目の攻撃時の音")]
    public AudioClip _attack_2;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_2;
    [SerializeField, Header("音量"), Range(0f, 2f)]
    public float _audioVolume_2;

    [SerializeField, Header("３回目の攻撃時の音")]
    public AudioClip _attack_3;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_3;
    [SerializeField, Header("音量"), Range(0f, 2f)]
    public float _audioVolume_3;

    [SerializeField, Header("強めの攻撃時の音\n※ヒーラーは魔法陣から音が出るためここに設定しない")]
    public AudioClip _attack_Strong;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_Strong;
    [SerializeField, Header("音量"), Range(0f, 2f)]
    public float _audioVolume_Strong;

    [SerializeField, Header("回避時の音\n※タンクは盾を構える音")]
    public AudioClip _dodge;
    [SerializeField, Header("再生速度"), Range(0f, 2f)]
    public float _playBackSpeed_Dodge;
    [SerializeField, Header("音量"), Range(0f, 2f)]
    public float _audioVolume_Dodge;

}
