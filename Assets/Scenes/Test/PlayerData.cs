using Fusion;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{

    public GameObject[] _playerAvatar = default;

    [Networked]
    private int _avatarNumber { get; set; } = default;

    [Networked]
    public string _avatarName { get; set; } = default;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");

            CharacterSelectionManager characterSelectionManager = canvas.GetComponentInChildren<CharacterSelectionManager>();

            characterSelectionManager.ActiveUI();
            characterSelectionManager.SetPlayer(this);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ActiveAvatar()
    {
        _playerAvatar[_avatarNumber - 1].SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetAvatarInfo(int avatarNumber, string avatarName)
    {
        _avatarNumber = avatarNumber;
        _avatarName = avatarName;
    }

}
