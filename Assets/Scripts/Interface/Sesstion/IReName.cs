using Fusion;

/// <summary>
/// 名前を変更するためのインターフェース
/// </summary>
public interface IReName
{

    public void RPC_ParticipantReName(string newName, NetworkObject participant);

}
