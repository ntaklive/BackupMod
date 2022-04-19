using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class PlatformManager : IPlatformManager
{
    public PlatformUserIdentifierAbs GetInternalLocalUserIdentifier()
    {
        return Platform.PlatformManager.InternalLocalUserIdentifier;
    }
}