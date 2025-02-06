using UnityEngine;

[System.Serializable]
public struct CharacterSoundStruct
{

    [SerializeField, Header("���g�̃T�E���h�\�[�X")]
    public AudioSource _audioSource;
    [Space(20)]
    [SerializeField, Header("�P��ڂ̍U�����̉�")]
    public AudioClip _attack_1;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_1;
    [SerializeField, Header("����"), Range(0f, 1f)]
    public float _audioVolume_1;
    [SerializeField, Header("�x��"), Min(0f)]
    public float _delay_1;
    [Space(20)]
    [SerializeField, Header("�Q��ڂ̍U�����̉�")]
    public AudioClip _attack_2;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_2;
    [SerializeField, Header("����"), Range(0f, 1f)]
    public float _audioVolume_2;
    [SerializeField, Header("�x��"), Min(0f)]
    public float _delay_2;
    [Space(20)]
    [SerializeField, Header("�R��ڂ̍U�����̉�")]
    public AudioClip _attack_3;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_3;
    [SerializeField, Header("����"), Range(0f, 1f)]
    public float _audioVolume_3;
    [SerializeField, Header("�x��"), Min(0f)]
    public float _delay_3;
    [Space(20)]
    [SerializeField, Header("���߂̍U�����̉�")]
    public AudioClip _attack_Strong;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_Strong;
    [SerializeField, Header("����"), Range(0f, 1f)]
    public float _audioVolume_Strong;
    [SerializeField, Header("�x��"), Min(0f)]
    public float _delay_Strong;
    [Space(20)]
    [SerializeField, Header("�K�E�Z�̉�")]
    public AudioClip _attack_Special;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_Special;
    [SerializeField, Header("����"), Range(0f, 1f)]
    public float _audioVolume_Special;
    [SerializeField, Header("�x��"), Min(0f)]
    public float _delay_Special;
    [Space(20)]
    [SerializeField, Header("��e��")]
    public AudioClip _getHit;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_GetHit;
    [SerializeField, Header("����"), Range(0f, 1f)]
    public float _audioVolume_GetHit;
    [SerializeField, Header("�x��"), Min(0f)]
    public float _delay_GetHit;
    [Space(20)]
    [SerializeField, Header("������̉�\n���^���N�͏����\���鉹")]
    public AudioClip _dodge;
    [SerializeField, Header("�Đ����x"), Range(0f, 2f)]
    public float _playBackSpeed_Dodge;
    [SerializeField, Header("����"), Range(0f, 1f)]
    public float _audioVolume_Dodge;
    [SerializeField, Header("�x��"), Min(0f)]
    public float _delay_Dodge;

}

