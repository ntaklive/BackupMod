namespace NtakliveBackupMod.Scripts.Services.Abstractions;

public interface IWorldService
{
    public World GetCurrentWorld();

    public string GetCurrentWorldSaveDirectory();

    public string GetCurrentPlayerDataLocalDirectory();

    public string GetCurrentGetPlayerDataDirectory();
}