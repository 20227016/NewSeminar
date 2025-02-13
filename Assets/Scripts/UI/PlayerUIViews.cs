using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIViews
{
    private readonly Dictionary<Slider, System.IDisposable> _animationDisposables = new();
    private readonly HashSet<Slider> _initializedSliders = new(); // 初回チェック用

    public void UpdateGauge(Slider slider, float value, float animationSpeed)
    {

        // 初回のみスライダーの最大値を設定
        if (!_initializedSliders.Contains(slider))
        {
            slider.maxValue = value;
            slider.value = value; // 初期値を最大に設定
            _initializedSliders.Add(slider);
        }

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
}
