using UnityEngine;
/// <summary>
/// BGMを扱うスクリプト
/// </summary>
public class AudioManager : MonoBehaviour
{

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip[] _bgmClips;

    // 現在の BGM のインデックス(初期値0 = 集会所のBGM)
    private int _currentBGMIndex = 0;

    void Start()
    {
        PlayBGM(_currentBGMIndex);
    }

    /// <summary>
    /// 指定した BGM を再生
    /// </summary>
    /// <param name="index">BGM のインデックス (0,1,2)</param>
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= _bgmClips.Length) return; // 範囲外チェック

        _audioSource.clip = _bgmClips[index]; // BGM をセット
        _audioSource.Play(); // 再生
        _currentBGMIndex = index; // 現在の BGM を更新
    }

    /// <summary>
    /// 次の BGM に切り替え
    /// </summary>
    public void NextBGM()
    {
        int nextIndex = _currentBGMIndex;
        PlayBGM(nextIndex);
    }

    /// <summary>
    /// BGM の停止
    /// </summary>
    public void StopBGM()
    {
        _audioSource.Stop();
    }


    public void OnStageNormalBGM()
    {
        print("BGM変更 : ノーマル");
        _currentBGMIndex = 2;
        NextBGM();
    }

    public void OnStageBossBGM()
    {
        print("BGM変更 : ボス");
        _currentBGMIndex = 3;
        NextBGM();
    }

}
