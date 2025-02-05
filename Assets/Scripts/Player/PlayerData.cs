using Fusion;
using UnityEngine;
using TMPro;
using System.Linq;

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


    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            CharacterSelectionManager characterSelectionManager = canvas.GetComponentInChildren<CharacterSelectionManager>();
            _normalStageTransfer = FindObjectOfType<NormalStageTransfer>();
            _bossStageTransfer = Resources.FindObjectsOfTypeAll<BossStageTransfer>().FirstOrDefault(obj => obj.name == "IsHitPlayer");
            if (_normalStageTransfer == null)
            {

                Debug.Log("トランスファーがない");

            }       
            if (_bossStageTransfer == null)
            {

                Debug.Log("トランスファーがない");

            }
            _networkRunner = FindObjectOfType<NetworkRunner>();
            if (_networkRunner == null)
            {

                Debug.Log("ランナーがない");

            }
            // アクティブ
            characterSelectionManager.ActiveUI();

            // プレイヤーセット
            characterSelectionManager.SetPlayer(this);
        }
        RPC_PlayerJoined();

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {

        base.Despawned(runner, hasState);
        RPC_PlayerLeft();

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerJoined()
    {

        Debug.Log($"トランスファー（ノーマル）：{_normalStageTransfer}");
        Debug.Log($"ランナー：{_networkRunner}");
        _normalStageTransfer.StageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;
        Debug.Log($"現在の参加人数{_networkRunner.SessionInfo.PlayerCount}");
        _bossStageTransfer.BossStageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerLeft()
    {

        if (Runner.IsServer)
        {

            return;

        }
        _normalStageTransfer.StageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;
        _bossStageTransfer.BossStageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;

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
