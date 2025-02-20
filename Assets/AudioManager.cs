using UnityEngine;
/// <summary>
/// BGM�������X�N���v�g
/// </summary>
public class AudioManager : MonoBehaviour
{

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip[] _bgmClips;

    // ���݂� BGM �̃C���f�b�N�X(�����l0 = �W���BGM)
    private int _currentBGMIndex = 0;

    void Start()
    {
        PlayBGM(_currentBGMIndex);
    }

    /// <summary>
    /// �w�肵�� BGM ���Đ�
    /// </summary>
    /// <param name="index">BGM �̃C���f�b�N�X (0,1,2)</param>
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= _bgmClips.Length) return; // �͈͊O�`�F�b�N

        _audioSource.clip = _bgmClips[index]; // BGM ���Z�b�g
        _audioSource.Play(); // �Đ�
        _currentBGMIndex = index; // ���݂� BGM ���X�V
    }

    /// <summary>
    /// ���� BGM �ɐ؂�ւ�
    /// </summary>
    public void NextBGM()
    {
        int nextIndex = _currentBGMIndex;
        PlayBGM(nextIndex);
    }

    /// <summary>
    /// BGM �̒�~
    /// </summary>
    public void StopBGM()
    {
        _audioSource.Stop();
    }


    public void OnStageNormalBGM()
    {
        print("BGM�ύX : �m�[�}��");
        _currentBGMIndex = 2;
        NextBGM();
    }

    public void OnStageBossBGM()
    {
        print("BGM�ύX : �{�X");
        _currentBGMIndex = 3;
        NextBGM();
    }

}
