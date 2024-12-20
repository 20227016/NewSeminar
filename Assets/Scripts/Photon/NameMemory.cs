using Fusion;

public class NameMemory : NetworkBehaviour, INameMemory
{

    [Networked]
    public string Name { get; set; } = "";

    /// <summary>
    /// スポーンしたときに呼び出される
    /// </summary>
    public override void Spawned()
    {

        base.Spawned();
        // 中身があるとき
        if (Name != "")
        {

            this.gameObject.name = Name;


        }

    }

    /// <summary>
    /// テキストの更新
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NameUpdate()
    {

        this.gameObject.name = Name;

    }

}
