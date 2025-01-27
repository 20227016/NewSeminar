using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStart : MonoBehaviour
{
    [SerializeField] private float _detectionRadius = 40f; // 検知範囲
    [SerializeField] private LayerMask _playerLayer; // プレイヤーを検知するためのレイヤーマスク

    private bool isPlayerNearby = false; // プレイヤーが範囲内にいるかどうか

    private GameObject _movieCamera = default;
    private GameObject _roar = default;
    private GameObject _circle = default;
    private GameObject _text = default;
    private GameObject _bar = default;

    private void Awake()
    {
        _movieCamera = GameObject.Find("MovieCamera");
        _movieCamera.SetActive(false);
        _roar = GameObject.Find("RoarEffects");
        _roar.SetActive(false);
        _circle = GameObject.Find("Magic circle Enemy");
        _circle.SetActive(false);

        //　ボス召喚待ち
        StartCoroutine(Delay(5f));
    }

    /// <summary>
    /// 死亡状態
    /// </summary>
    private IEnumerator Delay(float fadeDuration)
    {
        yield return new WaitForSeconds(fadeDuration);

        _text = GameObject.Find("BossNameText");
        _text.SetActive(false);
        _bar = GameObject.Find("BossHP_Bar");
        _bar.SetActive(false);
    }

    private void Update()
    {
        if (!isPlayerNearby)
        {
            // プレイヤーが範囲内にいるかどうかをチェック
            CheckForPlayers();
            return;
        }
    }

    /// <summary>
    /// プレイヤーが範囲内にいるかどうかを検知
    /// </summary>
    private void CheckForPlayers()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius, _playerLayer);

        if (hitColliders.Length > 0)
        {
            // プレイヤーが1人以上範囲内にいる
            isPlayerNearby = true;
            _movieCamera.SetActive(true);
            _roar.SetActive(true);
            _circle.SetActive(true);
            _text.SetActive(true);
            _bar.SetActive(true);
        }
    }

    /// <summary>
    /// 検知範囲をシーンビューで可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
