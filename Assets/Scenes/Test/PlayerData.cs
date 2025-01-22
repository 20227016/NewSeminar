using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{

    public GameObject[] _playerAvatar = default;

    [Networked]
    private int _avatarNumber { get; set; } = default;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");

            CharacterSelectionManager _characterSelectionManager = canvas.GetComponentInChildren<CharacterSelectionManager>();

            _characterSelectionManager.SetPlayer(this);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ActiveAvatar()
    {
        _playerAvatar[_avatarNumber - 1].SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetAvatarNumber(int avatarNumber)
    {
        _avatarNumber = avatarNumber;
    }

}
