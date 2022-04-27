using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class ConnectionManagerProvider : IConnectionManagerProvider
{
    public ConnectionManager GetConnectionManager()
    {
        return SingletonMonoBehaviour<ConnectionManager>.Instance;
    }
}