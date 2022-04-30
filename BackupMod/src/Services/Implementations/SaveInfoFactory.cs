using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

public class SaveInfoFactory : ISaveInfoFactory
{
    private readonly IDirectoryService _directoryService;

    public SaveInfoFactory(
        IDirectoryService directoryService)
    {
        _directoryService = directoryService;
    }
    
    public SaveInfo CreateFromSaveFolderPath(string saveFolderPath)
    {
        string worldFolderPath = _directoryService.GetParentDirectoryPath(saveFolderPath);
        string saveName = _directoryService.GetDirectoryName(saveFolderPath);
        string worldName = _directoryService.GetDirectoryName(worldFolderPath);

        return new SaveInfo
        {
            SaveFolderPath = saveFolderPath,
            WorldFolderPath = worldFolderPath,
            SaveName = saveName,
            WorldName = worldName,
        };
    }
}