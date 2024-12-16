using UnityEngine;
using Fusion;

public interface  INameMemory
{

    public string Name { get; set; }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NameUpdate();

}