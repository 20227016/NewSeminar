using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NameMemory : NetworkBehaviour, INameMemory
{

    [Networked]
    public string Name { get; set; } = default;

    /// <summary>
    /// �X�|�[�������Ƃ��ɌĂяo�����
    /// </summary>
    public override void Spawned()
    {

        // ���g������Ƃ�
        if (Name != "")
        {

            this.name = Name;


        }

    }

    /// <summary>
    /// �e�L�X�g�̍X�V
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NameUpdate()
    {

        this.name = Name;

    }

}
