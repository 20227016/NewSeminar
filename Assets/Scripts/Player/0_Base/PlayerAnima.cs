
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

    public float TriggerAnimation(Animator animator, AnimationClip animationClip)
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

        animator.SetTrigger(animationClip.name + "Trigger");

        // アニメーションの再生時間を返す
        return animationClip.length;
    }

    public void BoolAnimation(Animator animator, AnimationClip animationClip, bool isPlay)
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

        animator.SetBool(animationClip.name, isPlay);
    }

    public float PlayAnimation(Animator animator, AnimationClip animationClip)
    {
        animator.Play(animationClip.name);

        // アニメーションの再生時間を返す
        return animationClip.length;
    }
}