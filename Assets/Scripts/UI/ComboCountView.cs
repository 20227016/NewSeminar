using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;

public class ComboCountView
{
    private CancellationTokenSource cancellationTokenSource;

    public async void UpdateText(int value, TextMeshProUGUI text)
    {
        text.text = value.ToString();
        text.gameObject.SetActive(value > 0);

        if (value > 0)
        {
            // 既存のアニメーションを中断
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            // 新しいアニメーションを開始
            await BounceText(text, cancellationTokenSource.Token);
        }
    }

    private async UniTask BounceText(TextMeshProUGUI text, CancellationToken cancellationToken)
    {
        text.transform.localScale = Vector3.one; // 初期サイズ

        // Bounce Animation
        for (int i = 0; i < 30; i++) // 30フレーム（約1秒間）
        {
            if (cancellationToken.IsCancellationRequested)
            {
                text.transform.localScale = Vector3.one; // 中断時に元のサイズに戻す
                return; // アニメーションを中断
            }

            float bounce = Mathf.Sin(Time.time * 10) * 0.2f + 1; // サイン波を使用
            text.transform.localScale = new Vector3(1, bounce, 1);
            await UniTask.Yield(); // 次のフレームを待機
        }

        text.transform.localScale = Vector3.one; // 元のサイズに戻す
    }
}