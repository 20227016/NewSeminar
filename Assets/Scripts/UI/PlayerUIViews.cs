using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;

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
    // 各ゲージごとにアニメーション用のIDisposableを管理
    private readonly Dictionary<Slider, IDisposable> _animationDisposables = new Dictionary<Slider, IDisposable>();

    public void UpdateGauge(Slider slider, float value, float animationSpeed)
    {
        // アニメーション中の場合は停止
        if (_animationDisposables.ContainsKey(slider))
        {
            _animationDisposables[slider]?.Dispose();
        }

        // アニメーションを開始
        _animationDisposables[slider] = Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                // ゲージを滑らかに更新
                slider.value = Mathf.Lerp(slider.value, value, Time.deltaTime * animationSpeed);

                // 目標値に到達したらアニメーションを終了
                if (Mathf.Abs(slider.value - value) < 0.01f)
                {
                    slider.value = value;
                    _animationDisposables[slider]?.Dispose();
                    _animationDisposables.Remove(slider);
                }
            });
    }

    public void UpdateGauge2(Slider slider, float value, float animationSpeed)
    {
        // アニメーション中の場合は停止
        if (_animationDisposables.ContainsKey(slider))
        {
            _animationDisposables[slider]?.Dispose();
        }
        Debug.Log(slider.name + value);
        // アニメーションを開始
        _animationDisposables[slider] = Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                // ゲージを滑らかに更新
                slider.value = Mathf.Lerp(slider.value, value, Time.deltaTime * animationSpeed);

                // 目標値に到達したらアニメーションを終了
                if (Mathf.Abs(slider.value - value) < 0.01f)
                {
                    slider.value = value;
                    _animationDisposables[slider]?.Dispose();
                    _animationDisposables.Remove(slider);
                }
            });
    }
}
