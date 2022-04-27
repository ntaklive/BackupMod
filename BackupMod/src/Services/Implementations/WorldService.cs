using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

public class WorldService : IWorldService
{
    private readonly IDirectoryService _directoryService;

    public WorldService(
        IDirectoryService directoryService)
    {
        _directoryService = directoryService;
    }
    
    public World GetCurrentWorld() => GameManager.Instance.World;
    public string GetCurrentWorldSaveDirectory() => GameIO.GetSaveGameDir();

    public string GetCurrentPlayerDataLocalDirectory() => GameIO.GetPlayerDataLocalDir();

    public string GetCurrentGetPlayerDataDirectory() => GameIO.GetPlayerDataDir();
    
    public SaveInfo GetCurrentWorldSaveInfo()
    {
        string saveFolderPath = GetCurrentWorldSaveDirectory();
        string worldFolderPath = _directoryService.GetParentDirectoryPath(saveFolderPath);
        string saveName = _directoryService.GetDirectoryName(saveFolderPath);
        string worldName = _directoryService.GetDirectoryName(worldFolderPath);

        var saveInfo = new SaveInfo
        {
            SaveFolderPath = saveFolderPath,
            WorldFolderPath = worldFolderPath,
            SaveName = saveName,
            WorldName = worldName,
        };

        return saveInfo;
    }
}