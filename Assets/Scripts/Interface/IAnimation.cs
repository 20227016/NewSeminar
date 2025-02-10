
using UnityEngine;

public interface IAnimation
{
    /// <summary>
    /// trigger����p
    /// </summary>
    /// <param name="animator">���삷��A�j���[�^�[</param>
    /// <param name="animationClip">���삷��A�j���[�V�����N���b�v</param>
    void TriggerAnimation(Animator animator, string animationClipName);

    /// <summary>
    /// bool����p
    /// </summary>
    /// <param name="animator">���삷��A�j���[�^�[</param>
    /// <param name="animationClip">���삷��A�j���[�V�����N���b�v</param>
    /// <param name="isPlay">�Đ����邩</param>
    void BoolAnimation(Animator animator, string animationClipName, bool isPlay);

    /// <summary>
    /// play����p
    /// </summary>
    /// <param name="animator">���삷��A�j���[�^�[</param>
    /// <param name="animationClip">���삷��A�j���[�V�����N���b�v</param>
    void PlayAnimation(Animator animator, string animationClipName);

    public float GetAnimationLength(Animator animator, string animationClipName);
}