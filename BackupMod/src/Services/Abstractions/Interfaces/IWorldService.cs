using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IWorldService
{
    public World GetCurrentWorld();

    public string GetCurrentWorldSaveFolderPath();

    public string GetCurrentPlayerDataLocalFolderPath();

    public string GetCurrentGetPlayerDataFolderPath();

    public SaveInfo GetCurrentWorldSaveInfo();
}