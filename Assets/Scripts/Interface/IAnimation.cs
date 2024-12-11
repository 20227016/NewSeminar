
using UnityEngine;

public interface IAnimation
{
    /// <summary>
    /// trigger操作用
    /// </summary>
    /// <param name="animator">操作するアニメーター</param>
    /// <param name="animationClip">操作するアニメーションクリップ</param>
    void TriggerAnimation(Animator animator, AnimationClip animationClip);

    /// <summary>
    /// bool操作用
    /// </summary>
    /// <param name="animator">操作するアニメーター</param>
    /// <param name="animationClip">操作するアニメーションクリップ</param>
    /// <param name="isPlay">再生するか</param>
    void BoolAnimation(Animator animator, AnimationClip animationClip, bool isPlay);

    /// <summary>
    /// play操作用
    /// </summary>
    /// <param name="animator">操作するアニメーター</param>
    /// <param name="animationClip">操作するアニメーションクリップ</param>
    void PlayAnimation(Animator animator, AnimationClip animationClip);
}