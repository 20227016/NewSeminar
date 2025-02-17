using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class NormalStageTransfer : NetworkBehaviour
{

    // ポータル起動を通知する
    private Subject<Unit> _portalStartSubject = new Subject<Unit>();
    public IObservable<Unit> OnPortalStart => _portalStartSubject;



    /// <summary>
    ///  テレポート内のプレイヤーを管理するリスト
    /// </summary>
    private List<GameObject> _playersInPortal = new List<GameObject>();

    /// <summary>
    /// ノーマルステージのポータルに入りステージ移動した後の位置
    /// </summary>
    private GameObject _normalTeleportPosition = default;
    /// <summary>
    /// ボスステージのポータルに入りステージ移動した後の位置
    /// </summary>
    private GameObject _bossTeleportPosition = default;

    [Tooltip("ノーマルステージのテレポート座標")]
    private Transform _normalStageteleportPos = default;

    [Tooltip("ボスステージのテレポート座標")]
    private Transform _bossStageteleportPos = default;

    /// <summary>
    /// 敵管理クラス
    /// </summary>
    private WaveClear _waveClear = default;

    /// <summary>
    /// ノーマルステージをクリアしたかのフラグ
    /// </summary>
    [Networked]
    public bool ClearNormalStage { get; set; } = false;

    /// <summary>
    /// ボスのデバックをするかのフラグ
    /// </summary>
    [SerializeField, Header("ボスのデバックをするかのフラグ")]
    private bool _isDebug = false;

    /// <summary>
    /// スポーンしたことがあるかのフラグ
    /// </summary>
    [Networked]
    public bool HasTeleport { get; set; } = false;

    [Header("ノーマルステージのスカイボックス")]
    private Material _normalStageSkyBox = default;

    [Header("ボスステージのスカイボックス")]
    private Material _bossStageSkyBox = default;

    /// <summary>
    /// ノーマルステージにテレポートするために必要な人数
    /// </summary>
    [Networked, Tooltip("ノーマルステージにテレポートするために必要な人数")]
    public int StageRequiredPlayers { get; set; }

    [SerializeField, Header("自身のサウンドソース")]
    private AudioSource _audioSource = default;
    [SerializeField, Header("テレポートの音")]
    private AudioClip _audioClip = default;

    private ISound _sound = new SoundManager();

    public override void Spawned()
    {

        _waveClear = FindObjectOfType<WaveClear>();

        _normalTeleportPosition = GameObject.Find("NormalTeleportPosition");
        _bossTeleportPosition = GameObject.Find("BossTeleportPosition");

        _normalStageSkyBox = Resources.Load<Material>("Day1");
        _bossStageSkyBox = Resources.Load<Material>("Sunset3");
        if (_normalStageSkyBox != null)
        {
            print("取得できたよ");
        }
        else
        {
            print("できてないよ");
        }

        _normalStageteleportPos = _normalTeleportPosition.transform;
        _bossStageteleportPos = _bossTeleportPosition.transform;

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
            print($"プレイヤーを検知。現在の人数は {_playersInPortal.Count}/{StageRequiredPlayers} です");
        }
        if (_isDebug)
        {

            BossTeleportAllPlayers();
            return;
        }
        // 必要人数が揃ったら全員をテレポート
        if ((_playersInPortal.Count >= StageRequiredPlayers) && !ClearNormalStage || HasTeleport && !ClearNormalStage)
        {
            NormalTeleportAllPlayers();
            if (Runner.IsServer)
            {
                ChecgeBool();
            }
        }
        // もうすでに見方がテレポートしているとき
        else if ((_playersInPortal.Count >= StageRequiredPlayers) && ClearNormalStage || HasTeleport && ClearNormalStage)
        {

            BossTeleportAllPlayers();

        }
    }

    /// <summary>
    /// テレポートしたときにフラグを変更する
    /// </summary>
    private void ChecgeBool()
    {

        Debug.Log("Bool変更");
        HasTeleport = true;

    }

    /// <summary>
    /// 転送ポータルからプレイヤーが抜けたときにリストから削除する
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player") && _playersInPortal.Contains(collider.gameObject))
        {
            _playersInPortal.Remove(collider.gameObject);
            print($"<color=red>プレイヤーがポータルを離れました。現在の人数は {_playersInPortal.Count} です</color>");
        }
    }

    /// <summary>
    /// 全てのプレイヤーをテレポートさせる
    /// </summary>
    private void NormalTeleportAllPlayers()
    {
        // ノーマルステージにテレポート
        foreach (GameObject player in _playersInPortal)
        {

            // ポータル起動を通知する
            _portalStartSubject.OnNext(Unit.Default); // イベント発行
            print("イベント発火させたよ");

            player.transform.position = _normalStageteleportPos.position;
            print($"{player.name} をノーマルステージにテレポートしました");
        }
        Debug.Log($"<color=red>スカイボックス</color>{_normalStageSkyBox}");
        RenderSettings.skybox = _normalStageSkyBox;
        _sound.ProduceSE(_audioSource, _audioClip, 1, 1, 0);

    }

    private void BossTeleportAllPlayers()
    {
        // ボスステージにテレポート
        foreach (GameObject player in _playersInPortal)
        {

            player.transform.position = _bossStageteleportPos.position;
            print($"{player.name} をボスステージに途中参加としてテレポートしました");

        }
        RenderSettings.skybox = _bossStageSkyBox;
        _sound.ProduceSE(_audioSource, _audioClip, 1, 1, 0);
    }


    private void HandleAllEnemiesDefeated()
    {
        RPC_AllEnemiesDefeated();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_AllEnemiesDefeated()
    {
        _bossTeleportPosition = GameObject.Find("BossTeleportPosition");
        _bossStageteleportPos = _bossTeleportPosition.transform;
        ClearNormalStage = true;
        print("敵全滅の通知を受け取りました" + ClearNormalStage);
    }
}
