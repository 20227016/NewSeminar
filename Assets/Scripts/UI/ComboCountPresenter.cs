
using UnityEngine;
using TMPro;
using UniRx;
using Cysharp.Threading.Tasks;

/// <summary>
/// ComboCountPresenter.cs
/// クラス説明
/// 
///
/// 作成日: 9/27
/// 作成者: 山田智哉
/// </summary>
public class ComboCountPresenter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _comboCountText = default;

    [SerializeField]
    private TextMeshProUGUI _comboMultiplierText = default;

    /// <summary>
    /// 更新前処理
    /// </summary>
    private async void Start()
    {
        await UniTask.WaitUntil(() => ComboCounter.Instance != null);

        ComboCounter comboCounter = ComboCounter.Instance;
        ComboCountView comboCountView = new ComboCountView();

        // コンボ数を購読
        comboCounter.ComboReactiveProperty
            .DistinctUntilChanged()
            .Subscribe(value => comboCountView.UpdateText(value, _comboCountText, 0));

        // コンボ倍率を購読
        comboCounter.ComboReactiveProperty
            .DistinctUntilChanged()
            .Subscribe(value => comboCountView.UpdateText(comboCounter.GetComboMultiplier(), _comboMultiplierText, 1));

    }

}