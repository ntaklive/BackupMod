using System.Linq;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

public class SaveInfoFactory : ISaveInfoFactory
{
    private readonly IDirectoryService _directoryService;
    private readonly ISavesProvider _savesProvider;

    public SaveInfoFactory(
        IDirectoryService directoryService,
        ISavesProvider savesProvider)
    {
        _directoryService = directoryService;
        _savesProvider = savesProvider;
    }

    public SaveInfo GetFromSaveFolderPath(string saveFolderPath)
    {
        string worldFolderPath = _directoryService.GetParentDirectoryPath(saveFolderPath);
        
        WorldInfo world = _savesProvider.GetAllWorlds().First(world => world.WorldFolderPath == worldFolderPath);
        
        return world.Saves.First(save => save.SaveFolderPath == saveFolderPath);
    }
}