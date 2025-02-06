using UnityEngine;

[System.Serializable]
public struct CharacterSoundStruct
{

    [SerializeField, Header("©g‚ÌƒTƒEƒ“ƒhƒ\[ƒX")]
    public AudioSource _audioSource;
    [Space(20)]
    [SerializeField, Header("‚P‰ñ–Ú‚ÌUŒ‚‚Ì‰¹")]
    public AudioClip _attack_1;
    [SerializeField, Header("Ä¶‘¬“x"), Range(0f, 2f)]
    public float _playBackSpeed_1;
    [SerializeField, Header("‰¹—Ê"), Range(0f, 1f)]
    public float _audioVolume_1;
    [SerializeField, Header("’x‚ê"), Min(0f)]
    public float _delay_1;
    [Space(20)]
    [SerializeField, Header("‚Q‰ñ–Ú‚ÌUŒ‚‚Ì‰¹")]
    public AudioClip _attack_2;
    [SerializeField, Header("Ä¶‘¬“x"), Range(0f, 2f)]
    public float _playBackSpeed_2;
    [SerializeField, Header("‰¹—Ê"), Range(0f, 1f)]
    public float _audioVolume_2;
    [SerializeField, Header("’x‚ê"), Min(0f)]
    public float _delay_2;
    [Space(20)]
    [SerializeField, Header("‚R‰ñ–Ú‚ÌUŒ‚‚Ì‰¹")]
    public AudioClip _attack_3;
    [SerializeField, Header("Ä¶‘¬“x"), Range(0f, 2f)]
    public float _playBackSpeed_3;
    [SerializeField, Header("‰¹—Ê"), Range(0f, 1f)]
    public float _audioVolume_3;
    [SerializeField, Header("’x‚ê"), Min(0f)]
    public float _delay_3;
    [Space(20)]
    [SerializeField, Header("‹­‚ß‚ÌUŒ‚‚Ì‰¹")]
    public AudioClip _attack_Strong;
    [SerializeField, Header("Ä¶‘¬“x"), Range(0f, 2f)]
    public float _playBackSpeed_Strong;
    [SerializeField, Header("‰¹—Ê"), Range(0f, 1f)]
    public float _audioVolume_Strong;
    [SerializeField, Header("’x‚ê"), Min(0f)]
    public float _delay_Strong;
    [Space(20)]
    [SerializeField, Header("•KE‹Z‚Ì‰¹")]
    public AudioClip _attack_Special;
    [SerializeField, Header("Ä¶‘¬“x"), Range(0f, 2f)]
    public float _playBackSpeed_Special;
    [SerializeField, Header("‰¹—Ê"), Range(0f, 1f)]
    public float _audioVolume_Special;
    [SerializeField, Header("’x‚ê"), Min(0f)]
    public float _delay_Special;
    [Space(20)]
    [SerializeField, Header("”í’e‰¹")]
    public AudioClip _getHit;
    [SerializeField, Header("Ä¶‘¬“x"), Range(0f, 2f)]
    public float _playBackSpeed_GetHit;
    [SerializeField, Header("‰¹—Ê"), Range(0f, 1f)]
    public float _audioVolume_GetHit;
    [SerializeField, Header("’x‚ê"), Min(0f)]
    public float _delay_GetHit;
    [Space(20)]
    [SerializeField, Header("‰ñ”ğ‚Ì‰¹\n¦ƒ^ƒ“ƒN‚Í‚‚ğ\‚¦‚é‰¹")]
    public AudioClip _dodge;
    [SerializeField, Header("Ä¶‘¬“x"), Range(0f, 2f)]
    public float _playBackSpeed_Dodge;
    [SerializeField, Header("‰¹—Ê"), Range(0f, 1f)]
    public float _audioVolume_Dodge;
    [SerializeField, Header("’x‚ê"), Min(0f)]
    public float _delay_Dodge;

}

