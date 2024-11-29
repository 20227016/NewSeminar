
using UniRx;
using UnityEngine;

/// <summary>
/// LockOnCursorPresenter.cs
/// クラス説明
/// ロックオンカーソルの仲介クラス
///
/// 作成日: 9/24
/// 作成者: 山田智哉
/// </summary>
public class LockOnCursorPresenter : MonoBehaviour
{
    private PlayerTargetting _player = default;

    private LockOnCursorView _lockOnCursorView = default;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {

        _lockOnCursorView = GetComponent<LockOnCursorView>();

        GameLauncher.Instance.OnPlayerJoin.Subscribe(_ => SetModel(_));
    }


    private void SetModel(GameObject player)
    {
        _player = player.GetComponentInChildren<PlayerTargetting>();

        _player.LockOnEvent.Subscribe(target => _lockOnCursorView.UpdateLockOnCursorTarget(target));
    }
}