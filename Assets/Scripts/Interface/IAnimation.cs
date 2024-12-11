
using UnityEngine;

public interface IAnimation
{
    /// <summary>
    /// trigger����p
    /// </summary>
    /// <param name="animator">���삷��A�j���[�^�[</param>
    /// <param name="animationClip">���삷��A�j���[�V�����N���b�v</param>
    void TriggerAnimation(Animator animator, AnimationClip animationClip);

    /// <summary>
    /// bool����p
    /// </summary>
    /// <param name="animator">���삷��A�j���[�^�[</param>
    /// <param name="animationClip">���삷��A�j���[�V�����N���b�v</param>
    /// <param name="isPlay">�Đ����邩</param>
    void BoolAnimation(Animator animator, AnimationClip animationClip, bool isPlay);

    /// <summary>
    /// play����p
    /// </summary>
    /// <param name="animator">���삷��A�j���[�^�[</param>
    /// <param name="animationClip">���삷��A�j���[�V�����N���b�v</param>
    void PlayAnimation(Animator animator, AnimationClip animationClip);
}