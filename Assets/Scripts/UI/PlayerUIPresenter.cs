
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// PlayerUIPresenter.cs
/// クラス説明
///
///
/// 作成日: 9/10
/// 作成者: 山田智哉
/// </summary>
public class PlayerUIPresenter : MonoBehaviour
{
    // Viewクラス
    private PlayerUIViews _playerUIViews = new PlayerUIViews();

    [SerializeField, Tooltip("HPゲージ")]
    private Slider _hpGauge = default;

    [SerializeField, Tooltip("スタミナゲージ")]
    private Slider _staminaGauge = default;

    [SerializeField, Tooltip("スキルポイントゲージ")]
    private Slider _skillPointGauge = default;

    [SerializeField, Tooltip("ゲージ変動アニメーション速度")]
    private float _animationSpeed = 10f;

    /// <summary>
    /// UIにモデルをセットする
    /// </summary>
    /// <param name="player">セットするプレイヤー</param>
    public void SetModel(GameObject player)
    {
        // キャラクターをセット
        CharacterBase thisPlayer = player.GetComponentInChildren<CharacterBase>();

        // HPの更新購読
        thisPlayer.CurrentHP.Subscribe(value => _playerUIViews.UpdateGauge(_hpGauge, value, _animationSpeed));

        // スタミナの更新購読
        thisPlayer.CurrentStamina.Subscribe(value => _playerUIViews.UpdateGauge(_staminaGauge, value, _animationSpeed));

        // スキルポイントの更新購読
        thisPlayer.CurrentSkillPoint.Subscribe(value => _playerUIViews.UpdateGauge(_skillPointGauge, value, _animationSpeed));
    }
}