using Fusion;
using System.Threading.Tasks;

public interface IParticipantsSpawner
{

    public Task<bool> Spawner(NetworkRunner _networkRunner, RoomInfo preDefinedRoom);

}