using System.Linq;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

public class SaveInfoFactory : ISaveInfoFactory
{
    private readonly IDirectoryService _directoryService;
    private readonly IGameDataProvider _gameDataProvider;

    public SaveInfoFactory(
        IDirectoryService directoryService,
        IGameDataProvider gameDataProvider)
    {
        _directoryService = directoryService;
        _gameDataProvider = gameDataProvider;
    }

    public SaveInfo GetFromSaveFolderPath(string saveFolderPath)
    {
        string worldFolderPath = _directoryService.GetParentDirectoryPath(saveFolderPath);

#if DEBUG
        Log.Warning($"{nameof(GameIO.GetSaveGameDir)}:{GameIO.GetSaveGameDir()}");
        Log.Warning($"{nameof(GameIO.GetUserGameDataDir)}:{GameIO.GetUserGameDataDir()}");
        Log.Warning($"{nameof(GameIO.GetApplicationPath)}:{GameIO.GetApplicationPath()}");
        Log.Warning($"{nameof(GameIO.GetDocumentPath)}:{GameIO.GetDocumentPath()}");
        Log.Warning($"{nameof(GameIO.GetWorldDir)}:{GameIO.GetWorldDir()}");

        Log.Warning("World folder path: " + worldFolderPath);
        Log.Warning("Save folder path: " + saveFolderPath);
        Log.Warning("-----------");
        foreach (WorldInfo allWorld in _gameDataProvider.GetWorldsData())
        {
            Log.Warning("Available worlds:");
            Log.Warning("World name: " + allWorld.WorldName);
            Log.Warning("World folder path: " + allWorld.WorldFolderPath);
            foreach (SaveInfo save in allWorld.Saves)
            {
                Log.Warning("Available saves:");
                Log.Warning("  Save name: " + save.SaveName);
                Log.Warning("  Save folder path: " + save.SaveFolderPath);
                Log.Warning("  Backups folder path: " + save.BackupsFolderPath);
                Log.Warning("   Available backups:");
                foreach (BackupInfo backup in save.Backups)
                    Log.Warning("      Backup file path: " + backup.Filepath);
            }
        }
#endif

        WorldInfo world = _gameDataProvider.GetWorldsData().First(world => world.WorldFolderPath == worldFolderPath);
        
        return world.Saves.First(save => save.SaveFolderPath == saveFolderPath);
    }
}