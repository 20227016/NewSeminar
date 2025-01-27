using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovieBar : MonoBehaviour
{
    private Animator _animator = default;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void AnimatorFinished()
    {
        _animator.enabled = false;
    }
}
