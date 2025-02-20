using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
using UniRx;
using System;

public class BossStageTransfer : NetworkBehaviour
{
    // テレポート内のプレイヤーを管理するリスト
    private List<GameObject> _playersInPortal = new List<GameObject>();

    // 必要なプレイヤー数
    [ Tooltip("ノーマルステージにテレポートするために必要な人数")]
    public int BossStageRequiredPlayers { get; set; }// 必要なプレイヤー数
  

    [Tooltip("ボスステージのテレポート座標OBJ")]
    private GameObject _bossTeleportPosOBJ = default;

    [Tooltip("ボスステージのテレポート座標")]
    private Transform _bossTeleportPos = default;

    [SerializeField, Header("ボスステージのスカイボックス")]
    private Material _bossStageSkyBox = default;

    [SerializeField, Header("自身のサウンドソース")]
    private AudioSource _audioSource = default;
    [SerializeField, Header("テレポートの音")]
    private AudioClip _audioClip = default;

    [SerializeField]
    private AudioManager _audioManager = default;

    private ISound _sound = new SoundManager();

    public override void Spawned()
    {
        print("ボスのテレポーターがうまれました");
        _bossTeleportPosOBJ = GameObject.Find("BossTeleportPosition");
        _bossTeleportPos = _bossTeleportPosOBJ.transform;

        _audioManager =　FindObjectOfType<AudioManager>();
    }

    /// <summary>
    /// 転送ポータルにプレイヤーが入ったときにリストに追加する
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !_playersInPortal.Contains(collider.gameObject))
        {
            _playersInPortal.Add(collider.gameObject);
            print($"プレイヤーを検知。現在の人数は {_playersInPortal.Count} /{BossStageRequiredPlayers}です");
        }

        // 必要人数が揃ったら全員をテレポート
        if (_playersInPortal.Count >= BossStageRequiredPlayers)
        {
            BossTeleportAllPlayers();
        }
    }

    private void BossTeleportAllPlayers()
    {
        // ボスステージにテレポート
        foreach (GameObject player in _playersInPortal)
        {

            player.transform.position = _bossTeleportPos.position;
            print($"{player.name} をボス部屋にテレポートしました");
        }
        RenderSettings.skybox = _bossStageSkyBox;
        _sound.ProduceSE(_audioSource, _audioClip, 1, 1, 0);

        _audioManager.OnStageBossBGM();
    }
}
