
using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerAnima.cs
/// クラス説明
///
///
/// 作成日: 10/02
/// 作成者: 山田智哉
/// </summary>
public class PlayerAnima : IAnimation
{

    public void TriggerAnimation(Animator animator, string animationClipName)
    {
        // パラメーターを配列に取得
        AnimatorControllerParameter[] parameters = animator.parameters;

        // 各パラメーターを調べてBool型の場合、リセットする
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, false);
            }
        }

        animator.SetTrigger(animationClipName + "Trigger");
    }

    public void BoolAnimation(Animator animator, string animationClipName, bool isPlay)
    {
        animator.SetBool(animationClipName, isPlay);
    }

    public void PlayAnimation(Animator animator, string animationClipName)
    {
        animator.Play(animationClipName);
    }


    public float GetAnimationLength(Animator animator, string animationClipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animationClipName)
            {
                return clip.length;
            }
        }
        return 0f;
    }
}