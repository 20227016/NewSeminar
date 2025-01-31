using Fusion;
using UnityEngine;
using TMPro;

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
    private int _avatarNumber { get; set; } = default;

    [Networked, HideInInspector]
    public string _avatarName { get; set; } = default;


    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            CharacterSelectionManager characterSelectionManager = canvas.GetComponentInChildren<CharacterSelectionManager>();
            _normalStageTransfer = FindObjectOfType<NormalStageTransfer>();
            _bossStageTransfer = FindObjectOfType<BossStageTransfer>();
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

        _normalStageTransfer.NormalStageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;
        _bossStageTransfer.BossStageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerLeft()
    {

        _normalStageTransfer.NormalStageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;
        _bossStageTransfer.BossStageRequiredPlayers = _networkRunner.SessionInfo.PlayerCount;

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ActiveAvatar()
    {
        if (_avatarNumber <= 0)
        {
            return;
        }

        _playerAvatar[_avatarNumber - 1].SetActive(true);

        _playerNameTexts[_avatarNumber - 1].text = _avatarName;

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetAvatarInfo(int avatarNumber, string avatarName)
    {

        _avatarNumber = avatarNumber;


        _avatarName = avatarName;
    }

}
