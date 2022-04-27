using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IWorldService
{
    public World GetCurrentWorld();

    public string GetCurrentWorldSaveDirectory();

    public string GetCurrentPlayerDataLocalDirectory();

    public string GetCurrentGetPlayerDataDirectory();

    public SaveInfo GetCurrentWorldSaveInfo();
}