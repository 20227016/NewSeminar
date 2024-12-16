using Fusion;

public interface  IRoomController
{


    public void ParticipantAdd(PlayerRef playerRef);

    public void ParticipantRemove(PlayerRef playerRef);

    public void RPC_ParticipantReName(string name, NetworkObject participantsObj);

}
