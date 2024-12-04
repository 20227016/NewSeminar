using Fusion;
using System.Threading.Tasks;

public interface IParticipantsSpawner
{

    public Task<bool> Spawner(StartGameArgs startGameArgs, NetworkRunner _networkRunner);

}