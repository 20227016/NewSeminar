
using UnityEngine;
using UniRx;
using UnityEngine.UI;

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
    // モデルのプレイヤー
    private CharacterBase _player = default;

    // Viewクラス
    private PlayerUIViews _playerUIViews = new PlayerUIViews();

    [SerializeField]
    private Slider _hpGauge = default;

    [SerializeField]
    private Slider _staminaGauge = default;

    [SerializeField]
    private float _animationSpeed = 10f;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        GameLauncher.Instance.OnPlayerJoin.Subscribe(_ => SetModel(_));
    }

    private void SetModel(GameObject player)
    {
        try
        {
            _player = player.GetComponentInChildren<CharacterBase>();
        }
        catch
        {
            Debug.LogWarning("プレイヤーがおらへん");
        }
        Debug.Log(player.name);
        // HPの更新
        _player.CurrentHP.Subscribe(value => _playerUIViews.UpdateGauge(_hpGauge, value, _animationSpeed));

        // スタミナの更新
        _player.CurrentStamina.Subscribe(value => _playerUIViews.UpdateGauge(_staminaGauge, value, _animationSpeed));
    }
}