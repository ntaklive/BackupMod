using NtakliveBackupMod.Services.Abstractions.Models;

namespace NtakliveBackupMod.Services.Abstractions;

public interface IWorldService
{
    public World GetCurrentWorld();

    public string GetCurrentWorldSaveDirectory();

    public string GetCurrentPlayerDataLocalDirectory();

    public string GetCurrentGetPlayerDataDirectory();

    public SaveInfo GetCurrentWorldSaveInfo();
}