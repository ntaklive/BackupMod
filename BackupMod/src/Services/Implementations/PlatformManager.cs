using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class PlatformManager : IPlatformManager
{
    public PlatformUserIdentifierAbs GetInternalLocalUserIdentifier()
    {
        return Platform.PlatformManager.InternalLocalUserIdentifier;
    }
}