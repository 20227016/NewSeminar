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
        Debug.Log("��������������");


        _playerAvatar[_avatarNumber - 1].SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetAvatarNumber(int avatarNumber)
    {
        _avatarNumber = avatarNumber;
    }

        // �N���������Ă����^�C�~���O�Ǝ������L�����N�^�[��I�������^�C�~���O�łQ��B��̊֐����Ăׂ΂���
        // ������True�ɂł��邵�A�����ȊO�̐l���Q�������Ƃ��ɂ�True�ɂȂ邩��A�ォ������ė����l��������������悤�ɂȂ�
}
