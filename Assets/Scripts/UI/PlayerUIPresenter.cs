using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// PlayerUIPresenter.cs
/// プレイヤーのHP、スタミナ、スキルポイントなどをUIに反映するクラス。
///
/// 作成日: 9/10
/// 作成者: 山田智哉
/// </summary>
public class PlayerUIPresenter : MonoBehaviour
{
    // Viewクラス
    private PlayerUIViews _playerUIViews = new PlayerUIViews();

    [SerializeField, Tooltip("プレイヤーUIイメージ")]
    private Image[] _playerUIImages = default;

    [SerializeField, Tooltip("スキルアイコン")]
    private Sprite[] _skillIcons = default;

    [SerializeField, Tooltip("名前")]
    private TextMeshProUGUI[] _nameTexts = default;

    [SerializeField, Tooltip("HPゲージ")]
    private Slider[] _hpGauges = default;

    [SerializeField, Tooltip("スタミナゲージ")]
    private Slider _staminaGauge = default;

    [SerializeField, Tooltip("スキルポイントゲージ")]
    private Slider _skillPointGauge = default;

    [SerializeField, Tooltip("ゲージ変動アニメーション速度")]
    private float _animationSpeed = 10f;

    [SerializeField, Tooltip("キーコンフィグ")]
    private GameObject _keyconfig = default;

    [SerializeField, Tooltip("スキルアイコンイメージ")]
    private Image _skillIconImage = default;

    // 追跡用リスト（登録済みのプレイヤーキャラクター）
    private readonly List<CharacterBase> _registeredAllyModels = new();

    /// <summary>
    /// 自分自身のモデルをUIに設定する
    /// </summary>
    /// <param name="player">自分のプレイヤーオブジェクト</param>
    public void SetMyModel(GameObject player)
    {
        // キャラクターをセット
        CharacterBase thisPlayer = player.GetComponentInChildren<CharacterBase>();

        // HP1の更新購読
        thisPlayer.CurrentHP.Subscribe(value =>
            _playerUIViews.UpdateGauge(_hpGauges[0], value, _animationSpeed));

        // スタミナの更新購読
        thisPlayer.CurrentStamina.Subscribe(value =>
            _playerUIViews.UpdateGauge(_staminaGauge, value, _animationSpeed));

        // スキルポイントの更新購読
        thisPlayer.CurrentSkillPoint.Subscribe(value =>
            _playerUIViews.UpdateGauge(_skillPointGauge, value, _animationSpeed));

        // 名前を設定
        PlayerData playerData = thisPlayer.GetComponentInParent<PlayerData>();

        _nameTexts[0].text = playerData.AvatarName;

        // UIを有効化
        _playerUIImages[0].gameObject.SetActive(true);
        _keyconfig.SetActive(true);
        _skillIconImage.sprite = _skillIcons[playerData.AvatarNumber - 1];
    }

    /// <summary>
    /// 仲間のモデルをUIに設定する
    /// </summary>
    /// <param name="character">設定するキャラクター</param>
    /// <param name="modelCount">UIに割り当てるゲージのインデックス</param>
    public void SetAllyModel(CharacterBase character, int modelCount)
    {
        // すでに登録済みの場合はスキップ
        if (_registeredAllyModels.Contains(character))
        {
            return;
        }

        // 新規登録
        _registeredAllyModels.Add(character);

        // HPの更新購読
        character.CurrentHP.Subscribe(value =>
            _playerUIViews.UpdateGauge(_hpGauges[modelCount], value, _animationSpeed));

        // 名前を設定
        PlayerData playerData = character.GetComponentInParent<PlayerData>();

        _nameTexts[modelCount].text = playerData.AvatarName;

        // UIを有効化
        _playerUIImages[modelCount].gameObject.SetActive(true);
    }

    /// <summary>
    /// 全ての登録をクリアする
    /// </summary>
    public void ClearAllyModels()
    {
        _registeredAllyModels.Clear();
    }

    /// <summary>
    /// 登録済みのモデル数を取得
    /// </summary>
    /// <returns>登録済みのモデル数</returns>
    public int GetAllyModelCount()
    {
        return _registeredAllyModels.Count;
    }

    /// <summary>
    /// 指定のキャラクターが登録済みかを確認
    /// </summary>
    /// <param name="character">確認するキャラクター</param>
    /// <returns>登録済みならtrue</returns>
    public bool IsAllyModelSet(CharacterBase character)
    {
        return _registeredAllyModels.Contains(character);
    }
}