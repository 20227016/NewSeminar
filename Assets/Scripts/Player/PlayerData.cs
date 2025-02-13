using Fusion;
using UnityEngine;
using TMPro;
using System.Linq;
using UniRx;

public class PlayerData : NetworkBehaviour
{
    [SerializeField]
    private GameObject[] _playerAvatar = default;

    [SerializeField]
    private TextMeshProUGUI[] _playerNameTexts = default;

    /// <summary>
    /// ネットワークランナー
    /// </summary>
    private NetworkRunner _networkRunner = default;

    private NormalStageTransfer _normalStageTransfer = default;
    private BossStageTransfer _bossStageTransfer = default;

    [Networked]
    public int AvatarNumber { get; set; } = default;

    [Networked, HideInInspector]
    public string AvatarName { get; set; } = default;

    private EnemySpawner _enemySpawner = default;

    private int _playerCount = default;

    public override void Spawned()
    {

        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _normalStageTransfer = FindObjectOfType<NormalStageTransfer>();

        // 敵の全滅イベントを購読
        _enemySpawner.OnAllEnemiesDefeatedObservable.Subscribe(_ =>
        {
            Debug.Log("GameManager: 敵が全滅しました！ボス戦の準備を開始します。");
            HandleAllEnemiesDefeated();
        }).AddTo(this);

        if (_normalStageTransfer == null)
        {

            Debug.Log("トランスファーがない");

        }
        _networkRunner = FindObjectOfType<NetworkRunner>();
        if (_networkRunner == null)
        {

            Debug.Log("ランナーがない");

        }
        if (Object.HasInputAuthority)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            CharacterSelectionManager characterSelectionManager = canvas.GetComponentInChildren<CharacterSelectionManager>();
          
            // アクティブ
            characterSelectionManager.ActiveUI();

            // プレイヤーセット
            characterSelectionManager.SetPlayer(this);
        }

        RPC_PlayerJoined();

    }

    /// <summary>
    /// エネミーが全滅したら
    /// </summary>
    private void HandleAllEnemiesDefeated()
    {
        // 敵が全滅してからFindする。そして、プレイヤー人数を代入する
        _bossStageTransfer = Resources.FindObjectsOfTypeAll<BossStageTransfer>().FirstOrDefault(obj => obj.name == "BosteReporter");
        _bossStageTransfer.BossStageRequiredPlayers = _playerCount;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {

        base.Despawned(runner, hasState);
        RPC_PlayerLeft();

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerJoined()
    {

        _normalStageTransfer.StageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;
        Debug.Log($"現在の参加人数{_networkRunner.SessionInfo.PlayerCount}");
        _playerCount++;

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerLeft()
    {

        if (Runner.IsServer)
        {

            return;

        }
        _normalStageTransfer.StageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;
        _playerCount--;

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ActiveAvatar()
    {
        if (AvatarNumber <= 0)
        {
            return;
        }

        _playerAvatar[AvatarNumber - 1].SetActive(true);

        _playerNameTexts[AvatarNumber - 1].text = AvatarName;

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetAvatarInfo(int avatarNumber, string avatarName)
    {

        AvatarNumber = avatarNumber;


        AvatarName = avatarName;
    }

}
