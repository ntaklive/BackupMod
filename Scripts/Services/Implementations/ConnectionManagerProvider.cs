using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class ConnectionManagerProvider : IConnectionManagerProvider
{
    public ConnectionManager GetConnectionManager()
    {
        return SingletonMonoBehaviour<ConnectionManager>.Instance;
    }
}