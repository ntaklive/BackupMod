using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utils;

namespace BackupMod.Services;

public class WorldService : IWorldService
{
    private readonly ISaveInfoFactory _saveInfoFactory;

    public WorldService(ISaveInfoFactory saveInfoFactory)
    {
        _saveInfoFactory = saveInfoFactory;
    }
    
    public World GetCurrentWorld() => GameManager.Instance.World;
    public string GetCurrentWorldSaveFolderPath() => PathHelper.FixFolderPathSeparators(GameIO.GetSaveGameDir());

    public string GetCurrentPlayerDataLocalFolderPath() => PathHelper.FixFolderPathSeparators(GameIO.GetPlayerDataLocalDir());

    public string GetCurrentGetPlayerDataFolderPath() => PathHelper.FixFolderPathSeparators(GameIO.GetPlayerDataDir());
    
    public SaveInfo GetCurrentWorldSaveInfo() => _saveInfoFactory.CreateFromSaveFolderPath(GetCurrentWorldSaveFolderPath());
}