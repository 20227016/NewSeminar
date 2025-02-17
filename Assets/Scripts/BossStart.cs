using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

using System.Linq;
using UniRx;

public class BossStart : MonoBehaviour
{
    [SerializeField] private float _detectionRadius = 40f; // 検知範囲
    [SerializeField] private LayerMask _playerLayer; // プレイヤーを検知するためのレイヤーマスク

    private bool isPlayerNearby = false; // プレイヤーが範囲内にいるかどうか

    private GameObject _text = default;
    private GameObject _bar = default;

    private PlayableDirector _playableDirector; // TimelineのPlayableDirector

    private void Awake()
    {
        // MovieCamera
        // RoarEffects
        // Magic circle Enemy
        _playableDirector = GetComponent<PlayableDirector>();

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
            Debug.Log($"<color=red> ボスUI表示 </color>");
            _text = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "BossNameText");
            _bar = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "BossHP_Bar");
            if (_text != null)
            {
                _text.SetActive(false);
            }
            if (_bar != null)
            {
                _bar.SetActive(false);
            }
            // プレイヤーが1人以上範囲内にいる
            isPlayerNearby = true;
            _playableDirector.Play();
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
