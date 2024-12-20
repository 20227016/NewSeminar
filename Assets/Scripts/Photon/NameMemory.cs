using Fusion;

public class NameMemory : NetworkBehaviour, INameMemory
{

    [Networked]
    public string Name { get; set; } = "";

    /// <summary>
    /// �X�|�[�������Ƃ��ɌĂяo�����
    /// </summary>
    public override void Spawned()
    {

        base.Spawned();
        // ���g������Ƃ�
        if (Name != "")
        {

            this.gameObject.name = Name;


        }

    }

    /// <summary>
    /// �e�L�X�g�̍X�V
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NameUpdate()
    {

        this.gameObject.name = Name;

    }

}
