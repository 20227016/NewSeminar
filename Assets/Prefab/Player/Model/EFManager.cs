using System.Collections;
using UnityEngine;

public class EFManager : MonoBehaviour
{
    [SerializeField, Tooltip("プレイヤー")]
    private GameObject _player = default;

    [SerializeField, Tooltip("エフェクト")]
    private GameObject _ef = default;

    // プレイヤーの初期状態を保存
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector3 _initialScale;

    private void Start()
    {
        // 初期状態を保存
        _initialPosition = _player.transform.position;
        _initialRotation = _player.transform.rotation;
        _initialScale = _player.transform.localScale;
    }

    private void OnDisable()
    {
        ResetPlayerState();
        ResetEffect();
    }

    /// <summary>
    /// プレイヤーの位置、回転、大きさを初期状態に戻す
    /// </summary>
    private void ResetPlayerState()
    {
        _player.transform.position = _initialPosition;
        _player.transform.rotation = _initialRotation;
        _player.transform.localScale = _initialScale;
    }

    /// <summary>
    /// エフェクトを非アクティブ化してスケールをリセットする
    /// </summary>
    private void ResetEffect()
    {
        if (_ef != null)
        {
            _ef.SetActive(false);
            _ef.transform.localScale = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("エフェクトが設定されていません！");
        }
    }
}
