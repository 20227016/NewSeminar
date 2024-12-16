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
    /// スポーンしたときに呼び出される
    /// </summary>
    public override void Spawned()
    {

        // 中身があるとき
        if (Name != "")
        {

            this.name = Name;


        }

    }

    /// <summary>
    /// テキストの更新
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NameUpdate()
    {

        this.name = Name;

    }

}
