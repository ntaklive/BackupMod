namespace BackupMod.Services.Abstractions;

public interface IPlatformManager
{
    public PlatformUserIdentifierAbs GetInternalLocalUserIdentifier();
}