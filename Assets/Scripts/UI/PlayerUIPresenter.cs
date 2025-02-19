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

    [SerializeField, Tooltip("ロール")]
    private TextMeshProUGUI[] _rollName = default;

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

    [SerializeField, Tooltip("スキルクールタイムテキスト")]
    private TextMeshProUGUI _skillCoolTimeText = default;

    [SerializeField, Tooltip("回避アイコン")]
    private Image _avoidanceIconImage = default;

    [SerializeField, Tooltip("回避テキスト")]
    private TextMeshProUGUI _avoidanceText = default;

    [SerializeField, Tooltip("セッション名テキスト")]
    private TextMeshProUGUI _sessionNameText = default;

    private float _coolTimeRemaining = 0f;

    // クールタイムフラグ
    private bool _isCoolTimeActive = false;

    // 追跡用リスト（登録済みのプレイヤーキャラクター）
    private readonly List<CharacterBase> _registeredAllyModels = new();


    private void Start()
    {
        SessionManager sessionManager = FindObjectOfType<SessionManager>();

        if(sessionManager == null)
        {
            return;
        }

        if(sessionManager.SessionName == null)
        {
            return;
        }

        _sessionNameText.text = sessionManager.SessionName.ToString();
    }


    /// <summary>
    /// 自分自身のモデルをUIに設定する
    /// </summary>
    /// <param name="player">自分のプレイヤーオブジェクト</param>
    public void SetMyModel(GameObject player)
    {
        // キャラクターをセット
        CharacterBase thisPlayer = player.GetComponentInChildren<CharacterBase>();

        // HP1の更新購読
        thisPlayer.CurrentHP
            .Skip(1)
            .Subscribe(value =>
            _playerUIViews.UpdateGauge(_hpGauges[0], value, _animationSpeed));

        // スタミナの更新購読
        thisPlayer.CurrentStamina
            .Skip(1)
            .Subscribe(value =>
            _playerUIViews.UpdateGauge(_staminaGauge, value, _animationSpeed));

        // スキルポイントの更新購読
        thisPlayer.CurrentSkillPoint
            .Skip(1)
            .Subscribe(value =>
            _playerUIViews.UpdateGauge(_skillPointGauge, value, _animationSpeed));

        // スキルクールタイムの更新購読
        thisPlayer.SkillCoolTimeSubject.Subscribe(value =>
        {
            StartSkillCooldown(value);
        });

        // 名前を設定
        PlayerData playerData = thisPlayer.GetComponentInParent<PlayerData>();

        _nameTexts[0].text = playerData.AvatarName;

        // UIを有効化
        _playerUIImages[0].gameObject.SetActive(true);
        _keyconfig.SetActive(true);
        _skillIconImage.sprite = _skillIcons[playerData.AvatarNumber - 1];

        RollNameDisplay(_rollName[0], playerData.AvatarNumber);
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

        RollNameDisplay(_rollName[modelCount], playerData.AvatarNumber);

    }

    /// <summary>
    /// スキルクールタイムを開始する
    /// </summary>
    /// <param name="coolTime">クールタイムの時間</param>
    private void StartSkillCooldown(float coolTime)
    {
        _coolTimeRemaining = coolTime;
        _isCoolTimeActive = true;

        // スキルアイコンのアルファ値を下げる（透明度を上げる）
        Color iconColor = _skillIconImage.color;
        iconColor.a = 0.3f; // 透明度を50%に
        _skillIconImage.color = iconColor;

        _skillCoolTimeText.gameObject.SetActive(true);  // クールタイムテキストをアクティブ化

    }

    private void Update()
    {
        if (!_isCoolTimeActive)
        {
            return;
        }

        // 残り時間を減算
        _coolTimeRemaining -= Time.deltaTime;

        // クールタイムテキストを更新
        _skillCoolTimeText.text = Mathf.Max(0f, _coolTimeRemaining).ToString("F1");

        // クールタイムが終了したら非アクティブ化
        if (_coolTimeRemaining <= 0f)
        {
            _isCoolTimeActive = false;
            _skillCoolTimeText.gameObject.SetActive(false);  // クールタイムテキストを非アクティブ化
                                                             // スキルアイコンのアルファ値を元に戻す
            Color iconColor = _skillIconImage.color;
            iconColor.a = 1f; // 透明度100%（元の色）
            _skillIconImage.color = iconColor;
        }
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

    private void RollNameDisplay(TextMeshProUGUI rollText, int avatarNumber)
    {
        switch (avatarNumber)
        {
            // ノーマル（灰色）
            case 1:
                rollText.text = "ノーマル";
                rollText.color = Color.cyan;
                break;

            // ファイター（赤色）
            case 2:
                rollText.text = "ファイター";
                rollText.color = Color.red;
                break;

            // ヒーラー（黄緑）
            case 3:
                rollText.text = "ヒーラー";
                rollText.color = new Color(0.6f, 1f, 0.4f); // 黄緑
                break;

            // タンク（黄色）
            case 4:
                rollText.text = "タンク";
                rollText.color = Color.yellow;
                _avoidanceIconImage.sprite = _skillIcons[3];
                _avoidanceText.text = "ガード";
                break;

            // 選択されていないとき（ノーマル）
            default:
                rollText.text = "ノーマル";
                rollText.color = Color.cyan;
                break;
        }
    }
}