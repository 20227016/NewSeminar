
using UnityEngine;

public interface IAnimation
{
    void TriggerAnimation(Animator animator, string animationName);
    void BoolAnimation(Animator animator, string animationName, bool isPlay);

    void PlayAnimation(Animator animator, string animationName);
}