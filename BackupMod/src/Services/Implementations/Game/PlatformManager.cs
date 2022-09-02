using BackupMod.Services.Abstractions.Game;

namespace BackupMod.Services.Game;

public class PlatformManager : IPlatformManager
{
    public PlatformUserIdentifierAbs GetInternalLocalUserIdentifier()
    {
        return Platform.PlatformManager.InternalLocalUserIdentifier;
    }
}