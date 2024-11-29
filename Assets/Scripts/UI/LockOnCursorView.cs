
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// LockOnCursorView.cs
/// クラス説明
/// ロックオンカーソルの表示クラス
///
/// 作成日: 9/24
/// 作成者: 山田智哉
/// </summary>
public class LockOnCursorView : MonoBehaviour
{
    [SerializeField]
    private Image _lockOnCursor = default;

    private bool _isTargetting = false;

    private Transform _target = default;

    private Camera _camera = default;


    private void Awake()
    {
        _lockOnCursor.gameObject.SetActive(false);

        this.UpdateAsObservable()
            .Where(_ => _isTargetting)
            .Subscribe(_ =>
            {

                _lockOnCursor.transform.position = _camera.WorldToScreenPoint(_target.position);
            });

    }

    public void UpdateLockOnCursorPosition(Transform lockOnCursorTransform, Camera camera)
    {
        lockOnCursorTransform.position = camera.WorldToScreenPoint(_target.position);
    }


    public void UpdateLockOnCursorTarget(Transform targetTransform)
    {
        _camera = Camera.main;
        _target = targetTransform;
        _isTargetting = !_isTargetting;
        _lockOnCursor.gameObject.SetActive(_isTargetting);
    }
}