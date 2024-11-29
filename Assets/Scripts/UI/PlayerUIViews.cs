
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;

/// <summary>
/// PlayerUIViews.cs
/// クラス説明
/// プレイヤーUIのview
///
/// 作成日: 9/10
/// 作成者: 山田智哉
/// </summary>
public class PlayerUIViews
{
    private IDisposable _animationDisposable = default;


    public void UpdateGauge(Slider slider, float value,  float animationSpeed)
    {
        if (_animationDisposable != null)
        {
            _animationDisposable.Dispose();
        }

        _animationDisposable = Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                // ゲージを滑らかに減らす
                slider.value = Mathf.Lerp(slider.value, value, Time.deltaTime * animationSpeed);
                // 目標値に近づいたらアニメーションを終了する
                if (Mathf.Abs(slider.value - value) < 0.01f)
                {
                    slider.value = value;
                    _animationDisposable.Dispose();
                }
            });
    }
}