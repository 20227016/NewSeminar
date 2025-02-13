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
    /// �l�b�g���[�N�����i�[
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

        // �G�̑S�ŃC�x���g���w��
        _enemySpawner.OnAllEnemiesDefeatedObservable.Subscribe(_ =>
        {
            Debug.Log("GameManager: �G���S�ł��܂����I�{�X��̏������J�n���܂��B");
            HandleAllEnemiesDefeated();
        }).AddTo(this);

        if (_normalStageTransfer == null)
        {

            Debug.Log("�g�����X�t�@�[���Ȃ�");

        }
        _networkRunner = FindObjectOfType<NetworkRunner>();
        if (_networkRunner == null)
        {

            Debug.Log("�����i�[���Ȃ�");

        }
        if (Object.HasInputAuthority)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            CharacterSelectionManager characterSelectionManager = canvas.GetComponentInChildren<CharacterSelectionManager>();
          
            // �A�N�e�B�u
            characterSelectionManager.ActiveUI();

            // �v���C���[�Z�b�g
            characterSelectionManager.SetPlayer(this);
        }

        RPC_PlayerJoined();

    }

    /// <summary>
    /// �G�l�~�[���S�ł�����
    /// </summary>
    private void HandleAllEnemiesDefeated()
    {
        // �G���S�ł��Ă���Find����B�����āA�v���C���[�l����������
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
        Debug.Log($"���݂̎Q���l��{_networkRunner.SessionInfo.PlayerCount}");
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
