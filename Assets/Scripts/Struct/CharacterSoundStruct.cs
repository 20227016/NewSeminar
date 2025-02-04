using UnityEngine;

[System.Serializable]
public struct CharacterSoundStruct
{
    
     [SerializeField, Header("���g�̃T�E���h�\�[�X")]
    public AudioSource _audioSource;

    [SerializeField,Header("�P��ڂ̍U�����̉�")]
    public AudioClip _attack_1;
    [SerializeField, Header("�Đ����x"),Range(0f, 2f)]
    public float _playBackSpeed_1;    
    [SerializeField, Header("����"), Range(0f, 2f)]
    public float _audioVolume_1;

    [SerializeField, Header("�Q��ڂ̍U�����̉�")]
    public AudioClip _attack_2;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_2;
    [SerializeField, Header("����"), Range(0f, 2f)]
    public float _audioVolume_2;

    [SerializeField, Header("�R��ڂ̍U�����̉�")]
    public AudioClip _attack_3;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_3;
    [SerializeField, Header("����"), Range(0f, 2f)]
    public float _audioVolume_3;

    [SerializeField, Header("���߂̍U�����̉�\n���q�[���[�͖��@�w���特���o�邽�߂����ɐݒ肵�Ȃ�")]
    public AudioClip _attack_Strong;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_Strong;
    [SerializeField, Header("����"), Range(0f, 2f)]
    public float _audioVolume_Strong;

    [SerializeField, Header("������̉�\n���^���N�͏����\���鉹")]
    public AudioClip _dodge;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_Dodge;
    [SerializeField, Header("����"), Range(0f, 2f)]
    public float _audioVolume_Dodge;

}
