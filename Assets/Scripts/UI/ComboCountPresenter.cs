
using UnityEngine;
using TMPro;
using UniRx;

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

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        ComboCounter comboCounter = ComboCounter.Instance;
        ComboCountView comboCountView = new ComboCountView();
        comboCounter.ComboCount.Subscribe(value => comboCountView.UpdateText(value, _comboCountText));
    }

}