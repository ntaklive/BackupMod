using NtakliveBackupMod.Services.Abstractions;

namespace NtakliveBackupMod.Services.Implementations;

public class PlatformManager : IPlatformManager
{
    public PlatformUserIdentifierAbs GetInternalLocalUserIdentifier()
    {
        return Platform.PlatformManager.InternalLocalUserIdentifier;
    }
}