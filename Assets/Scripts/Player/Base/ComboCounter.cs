
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;

/// <summary>
/// ComboCounter.cs
/// クラス説明
/// コンボのシングルトンクラス
///
/// 作成日: 9/24
/// 作成者: 山田智哉
/// </summary>
public class ComboCounter : IComboCounter
{
    private static ComboCounter _instance;
    private ReactiveProperty<int> _comboCount;
    public IReadOnlyReactiveProperty<int> ComboCount => _comboCount;

    private CancellationTokenSource _comboResetCancellationTokenSource;

    // コンボリセットまでの時間（秒）
    private const float _comboResetTime = 10f;

    /// <summary>
    /// コンボカウンターのシングルトン化
    /// </summary>
    public static ComboCounter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ComboCounter();
                _instance.Initialize(); // 初期化処理を追加
            }

            return _instance;
        }
    }

    // コンストラクタで初期化
    private void Initialize()
    {
        _comboCount = new ReactiveProperty<int>(0); // ReactivePropertyの初期化
        _comboResetCancellationTokenSource = new CancellationTokenSource();
    }

    public void AddCombo()
    {
        // コンボ数を加算
        _comboCount.Value++;

        // 既存のリセット処理があればキャンセル
        _comboResetCancellationTokenSource.Cancel();
        _comboResetCancellationTokenSource = new CancellationTokenSource();

        // リセットのタイマーを開始
        StartComboResetTimerAsync(_comboResetCancellationTokenSource.Token).Forget();
    }

    // コンボ数を取得
    public int GetCombo()
    {
        return _comboCount.Value;
    }

    // コンボリセットのタイマー処理
    private async UniTaskVoid StartComboResetTimerAsync(CancellationToken token)
    {
        try
        {
            // 一定時間待つ（キャンセル可能）
            await UniTask.Delay(TimeSpan.FromSeconds(_comboResetTime), cancellationToken: token);

            // タイマーが完了したらコンボ数をリセット
            _comboCount.Value = 0;
        }
        catch (OperationCanceledException)
        {
            // タイマーがキャンセルされた場合は何もしない
        }
    }
}