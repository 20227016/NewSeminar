using UnityEngine;
using Cinemachine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections.Generic;

/// <summary>
/// PlayerTargetting.cs
/// クラス説明
/// プレイヤーのターゲッティングクラス
/// 
/// 作成日: 9/18
/// 作成者: 山田智哉
/// </summary>
public class PlayerTargetting : MonoBehaviour, ITargetting
{
    private const float FullCircleDegrees = 360f;
    private const float HalfCircleDegrees = 180f;
    private const float TargettingDistance = 50f;
    private const float TargettingFactor = 0.3f;
    private const float TargettingThreshold = 0.5f;
    private const float CameraRotationCorrectionFactor = Mathf.PI * 2;
    private const float DistanceNormalizationFactor = 500f;

    private CinemachineVirtualCamera _normalCamera = default;

    private CinemachineVirtualCamera _targettingCamera = default;

    [SerializeField, Tooltip("ターゲットのレイヤー")]
    private LayerMask _targetLayer = default;

    [SerializeField, Tooltip("無視するレイヤー")]
    private LayerMask _ignoreLayer = default;

    // 通常時カメラのPOVコンポーネント取得用
    private CinemachinePOV _normalCameraPOV = default;

    // メインカメラ
    private Camera _mainCamera = default;

    // ターゲッティングフラグ
    private bool _isTargetting = default;

    // 現在のターゲットオブジェクト
    private GameObject _currentTarget = default;

    // ロックオンイベント
    private Subject<Transform> _lockonEvent = new Subject<Transform>();

    public IObservable<Transform> LockOnEvent => _lockonEvent;


    public void InitializeSetting(Camera camera)
    {
        _mainCamera = camera;
        _normalCamera = GameObject.Find("NormalCamera").GetComponent<CinemachineVirtualCamera>();
        _targettingCamera = GameObject.Find("TargettingCamera").GetComponent<CinemachineVirtualCamera>();
        _normalCameraPOV = _normalCamera.GetCinemachineComponent<CinemachinePOV>();
        _targettingCamera.enabled = false;
        _isTargetting = false;

        //// 更新処理
        this.UpdateAsObservable()
            .Where(_ => _isTargetting && !IsTargetVisible(_currentTarget))
            .Subscribe(_ => Targetting());

        this.UpdateAsObservable()
            .Where(_ => _isTargetting && (_currentTarget == null || !IsTargetVisible(_currentTarget)))
            .Subscribe(_ => CancelTargetting());

        _normalCamera.Follow = transform;
        _normalCamera.LookAt = transform;
        _targettingCamera.Follow = transform;
    }

    private void CancelTargetting()
    {
        _isTargetting = false;
        _normalCamera.enabled = true;
        _targettingCamera.enabled = false;
        _currentTarget = null;
        _lockonEvent.OnNext(null); // ロックオン解除を通知
    }

    public void Targetting()
    {
        
        // ターゲッティングしてないとき
        if (!_isTargetting)
        {
            // 現在のターゲットにターゲット検索結果を格納
            _currentTarget = SearchTarget();
            if (_currentTarget == null) return;
            _targettingCamera.LookAt = _currentTarget.transform;
        }
        else
        {

            // ターゲッティングカメラの回転角をノーマルカメラのPOVに反映
            _normalCameraPOV.m_VerticalAxis.Value = Mathf.Repeat(_targettingCamera.transform.eulerAngles.x + HalfCircleDegrees, FullCircleDegrees) - HalfCircleDegrees;

            _normalCameraPOV.m_HorizontalAxis.Value = _targettingCamera.transform.eulerAngles.y;

        }
        if (_currentTarget == null) return;

        // カメラを切り替える
        _isTargetting = !_isTargetting;
        _normalCamera.enabled = !_isTargetting;
        _targettingCamera.enabled = _isTargetting;
        _lockonEvent.OnNext(_currentTarget.transform);
    }

    /// <summary>
    /// ターゲット視認フラグ
    /// </summary>
    /// <param name="target">ターゲットオブジェクト</param>
    /// <returns>ターゲットが視認できているか</returns>
    private bool IsTargetVisible(GameObject target)
    {
        if (target == null || target.gameObject == null) return false; // 破壊チェック

        Vector3 direction = target.transform.position - _mainCamera.transform.position;

        if (Physics.Raycast(_mainCamera.transform.position, direction, out RaycastHit hit, TargettingDistance, _ignoreLayer))
        {
            return hit.collider.gameObject.layer != _targetLayer;
        }

        return false;
    }

    /// <summary>
    /// ターゲットを検索
    /// </summary>
    private GameObject SearchTarget()
    {
        // まずは敵レイヤーのオブジェクトを検索
        RaycastHit[] enemyHits = Physics.SphereCastAll(transform.position, TargettingDistance, Vector3.up, 0, _targetLayer);

        if (enemyHits.Length == 0) return null;

        // 一番近い敵を見つける
        GameObject closestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var hit in enemyHits)
        {
            float distance = Vector3.Distance(transform.position, hit.collider.gameObject.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = hit.collider.gameObject;
            }
        }

        if (closestEnemy == null) return null;

        // 近い敵の子オブジェクトの中から targeting レイヤーを持つオブジェクトを探す
        Transform[] childTransforms = closestEnemy.GetComponentsInChildren<Transform>();
        foreach (var child in childTransforms)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Targeting"))
            {
                return child.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// ターゲットの角度を調整する
    /// </summary>
    private float AdjustAngle(float cameraAngle, float targetAngle, float distanceToTarget)
    {
        float adjustedAngle = cameraAngle - targetAngle;

        if (Mathf.PI <= adjustedAngle)
        {

            adjustedAngle -= CameraRotationCorrectionFactor;

        }
        else if (-Mathf.PI >= adjustedAngle)
        {

            adjustedAngle += CameraRotationCorrectionFactor;

        }

        return adjustedAngle + adjustedAngle * (distanceToTarget / DistanceNormalizationFactor) * TargettingFactor;
    }
}