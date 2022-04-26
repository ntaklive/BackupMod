using NtakliveBackupMod.Services.Abstractions;

namespace NtakliveBackupMod.Services.Implementations;

public class ConnectionManagerProvider : IConnectionManagerProvider
{
    public ConnectionManager GetConnectionManager()
    {
        return SingletonMonoBehaviour<ConnectionManager>.Instance;
    }
}