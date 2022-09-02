using BackupMod.Services.Abstractions.Game;

namespace BackupMod.Services.Game;

public class ConnectionManagerProvider : IConnectionManagerProvider
{
    public ConnectionManager GetConnectionManager()
    {
        return SingletonMonoBehaviour<ConnectionManager>.Instance;
    }
}