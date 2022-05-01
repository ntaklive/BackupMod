using System.Linq;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utils;

namespace BackupMod.Services;

public class SaveInfoFactory : ISaveInfoFactory
{
    private readonly IWorldInfoFactory _worldInfoFactory;
    private readonly IDirectoryService _directoryService;

    public SaveInfoFactory(
        IWorldInfoFactory worldInfoFactory,
        IDirectoryService directoryService)
    {
        _worldInfoFactory = worldInfoFactory;
        _directoryService = directoryService;
    }

    public SaveInfo CreateFromSaveFolderPath(string saveFolderPath)
    {
        string saveFolderPathFixed = PathHelper.FixFolderPathSeparators(saveFolderPath);
        
        WorldInfo world = _worldInfoFactory.CreateFromWorldFolderPath(_directoryService.GetParentDirectoryPath(saveFolderPathFixed));
        
        return world.Saves.First(save => save.SaveFolderPath == saveFolderPathFixed);
    }
}