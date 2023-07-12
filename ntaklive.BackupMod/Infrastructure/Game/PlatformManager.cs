namespace ntaklive.BackupMod.Infrastructure.Game
{
    public class PlatformManager : IPlatformManager
    {
        public PlatformUserIdentifierAbs GetInternalLocalUserIdentifier()
        {
            return Platform.PlatformManager.InternalLocalUserIdentifier;
        }
    }
}