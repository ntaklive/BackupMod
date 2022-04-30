using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

public class WorldService : IWorldService
{
    private readonly ISaveInfoFactory _saveInfoFactory;

    public WorldService(ISaveInfoFactory saveInfoFactory)
    {
        _saveInfoFactory = saveInfoFactory;
    }
    
    public World GetCurrentWorld() => GameManager.Instance.World;
    public string GetCurrentWorldSaveFolderPath() => GameIO.GetSaveGameDir();

    public string GetCurrentPlayerDataLocalFolderPath() => GameIO.GetPlayerDataLocalDir();

    public string GetCurrentGetPlayerDataFolderPath() => GameIO.GetPlayerDataDir();
    
    public SaveInfo GetCurrentWorldSaveInfo() => _saveInfoFactory.CreateFromSaveFolderPath(GetCurrentWorldSaveFolderPath());
}