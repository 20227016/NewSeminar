using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

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
        Debug.Log("あああああああ");


        _playerAvatar[_avatarNumber - 1].SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetAvatarNumber(int avatarNumber)
    {
        _avatarNumber = avatarNumber;
    }

        // 誰かが入ってきたタイミングと自分がキャラクターを選択したタイミングで２回。上の関数を呼べばいい
        // 自分もTrueにできるし、自分以外の人が参加したときにもTrueになるから、後から入って来た人も自分が見えるようになる
}
