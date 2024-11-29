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

        _normalCamera.Follow = transform;
        _normalCamera.LookAt = transform;
        _targettingCamera.Follow = transform;
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
        // ターゲットと自身の距離を計算
        Vector3 direction = target.transform.position - _mainCamera.transform.position;

        // 射線が通っているときはtrue
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
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, TargettingDistance, Vector3.up, 0, _targetLayer);

        if (hits.Length == 0) return null;

        List<GameObject> targetObjects = new List<GameObject>();

        foreach (var hit in hits)
        {

            Vector3 directionToHit = hit.collider.gameObject.transform.position - transform.position;

            if (Physics.Raycast(transform.position, directionToHit, out RaycastHit hitResult, TargettingDistance, _ignoreLayer))
            {

                if (hitResult.collider.gameObject.layer == hit.collider.gameObject.layer)
                {

                    // 射線上にいるターゲットをリストに追加
                    targetObjects.Add(hit.collider.gameObject);

                }

            }

        }

        if (targetObjects.Count == 0) return null;

        // 一番近いターゲットを選択
        float playerCameraForwardAngle = Mathf.Atan2(_mainCamera.transform.forward.x, _mainCamera.transform.forward.z);
        float closestAngle = CameraRotationCorrectionFactor;
        GameObject closestTarget = null;

        foreach (GameObject target in targetObjects)
        {

            Vector3 cameraToTarget = target.transform.position - _mainCamera.transform.position;
            Vector3 directionToTarget = cameraToTarget.normalized;

            float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z);

            targetAngle = AdjustAngle(playerCameraForwardAngle, targetAngle, cameraToTarget.magnitude);

            if (Mathf.Abs(closestAngle) >= Mathf.Abs(targetAngle))
            {

                closestAngle = targetAngle;
                closestTarget = target;

            }

        }

        return Mathf.Abs(closestAngle) <= TargettingThreshold ? closestTarget : null;
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
