using Cysharp.Threading.Tasks;
using Fusion;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

/// <summary>
/// ComboCounter.cs
/// Photon Fusion に対応したコンボカウントクラス
/// 作成日: 9/24
/// 作成者: 山田智哉
/// </summary>
public class ComboCounter : NetworkBehaviour, IComboCounter
{
    // シングルトン
    private static ComboCounter _instance;
    public static ComboCounter Instance => _instance;


    // コンボ数をネットワーク同期 ---------------------------------------------------------
    [Networked(OnChanged = nameof(OnComboCountChanged))]
    private int _networkComboCount { get; set; } // ネットワーク同期されるプロパティ

    private ReactiveProperty<int> _comboReactiveProperty = new(0);
    public IReadOnlyReactiveProperty<int> ComboReactiveProperty => _comboReactiveProperty;

    private static void OnComboCountChanged(Changed<ComboCounter> changed)
    {
        changed.Behaviour._comboReactiveProperty.Value = changed.Behaviour._networkComboCount;
    }
    // ------------------------------------------------------------------------------------

    // リセットタイマーのキャンセレーショントークン
    private CancellationTokenSource _comboResetCancellationTokenSource;

    // コンボリセットに要する時間
    private const float COMBO_RESET_TIME = 10f;


    private void Awake()
    {
        // シングルトン化
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        _comboResetCancellationTokenSource = new CancellationTokenSource();
    }

    public override void Spawned()
    {
        // 初期化
        if (Object.HasStateAuthority)
        {
            _networkComboCount = 0;
        }
    }

    /// <summary>
    /// コンボ加算
    /// </summary>
    public void AddCombo()
    {
        if (!Object.HasStateAuthority) return; // State Authority のみ加算可能

        // コンボを加算
        _networkComboCount++;

        // 既存のリセット処理があればキャンセル
        _comboResetCancellationTokenSource.Cancel();
        _comboResetCancellationTokenSource = new CancellationTokenSource();

        // コンボリセットタイマーを開始
        StartComboResetTimerAsync(_comboResetCancellationTokenSource.Token).Forget();
    }

    /// <summary>
    /// コンボリセットタイマー
    /// </summary>
    private async UniTaskVoid StartComboResetTimerAsync(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(COMBO_RESET_TIME), cancellationToken: token);

            // コンボ数をリセット
            if (Object.HasStateAuthority)
            {
                _networkComboCount = 0; 
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセル時は何もしない
        }
    }

    /// <summary>
    /// 現在のコンボ倍率を取得
    /// </summary>
    public float GetComboMultiplier()
    {
        // 10コンボ毎にコンボ倍率が0.1づつ上昇、最低等倍、最高2倍
        float comboMultiplier = Mathf.Clamp(1 + (_networkComboCount / 100), 1, 2);
        return comboMultiplier;
    }
}