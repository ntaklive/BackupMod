using System.Collections.Generic;
using System.IO;
using BackupMod.Extensions;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

public class SavesProvider : ISavesProvider
{
    private readonly IArchiveService _archiveService;
    private readonly IPathService _pathService;
    private readonly IDirectoryService _directoryService;
    private readonly IGameDirectoriesProvider _gameDirectoriesProvider;

    public SavesProvider(
        IArchiveService archiveService,
        IPathService pathService,
        IDirectoryService directoryService,
        IGameDirectoriesProvider gameDirectoriesProvider)
    {
        _archiveService = archiveService;
        _pathService = pathService;
        _directoryService = directoryService;
        _gameDirectoriesProvider = gameDirectoriesProvider;
    }

    public IEnumerable<WorldInfo> GetAllWorlds()
    {
        string allBackupsFolder = _gameDirectoriesProvider.GetBackupsFolderPath();
        string[] worldsBackupsPaths = _directoryService.GetDirectories(allBackupsFolder);

        foreach (string worldBackupFolderPath in worldsBackupsPaths)
        {
            string worldName = _directoryService.GetDirectoryName(worldBackupFolderPath);

            string worldFolderPath = _pathService.Combine(_gameDirectoriesProvider.GetSavesFolderPath(), worldName);
            var worldFolder = new DirectoryInfo(worldFolderPath);
            if (!worldFolder.Exists)
            {
                worldFolder.Create();
            }
            
            var saves = new List<SaveInfo>();
            foreach (string saveBackupFolderPath in _directoryService.GetDirectories(worldBackupFolderPath))
            {
                string saveName = _directoryService.GetDirectoryName(saveBackupFolderPath);

                string saveBackupsFolderPath = _pathService.Combine(_gameDirectoriesProvider.GetBackupsFolderPath(), worldName, saveName);
                var backupsFolder = new DirectoryInfo(saveBackupsFolderPath);
                if (!backupsFolder.Exists)
                {
                    backupsFolder.Create();
                }
                
                string saveFolderPath = _pathService.Combine(_gameDirectoriesProvider.GetSavesFolderPath(), worldName, saveName);
                var saveFolder = new DirectoryInfo(saveBackupsFolderPath);
                if (!saveFolder.Exists)
                {
                    saveFolder.Create();
                }

                var backups = new List<BackupInfo>();

                string[] backupZipFilePaths = _directoryService.GetFiles(saveBackupsFolderPath, $"{saveName}_*{_archiveService.Extension}", SearchOption.TopDirectoryOnly);
                foreach (string filepath in backupZipFilePaths)
                {
                    var fileInfo = new FileInfo(filepath);

                    var backupInfo = new BackupInfo
                    {
                        Filepath = filepath,
                        Timestamp = fileInfo.LastWriteTimeUtc,
                        SaveInfo = null
                    };

                    backups.Add(backupInfo);
                }

                var save = new SaveInfo
                {
                    SaveName = saveName,
                    SaveFolderPath = saveFolderPath,
                    BackupsFolderPath = saveBackupsFolderPath,
                    Backups = backups.ToArray(),
                    World = null,
                };

                save.Backups.ForEach(info => info.SaveInfo = save);

                saves.Add(save);
            }

            var world = new WorldInfo
            {
                WorldName = worldName,
                WorldFolderPath = worldFolderPath,
                Saves = saves.ToArray(),
            };

            world.Saves.ForEach(info => info.World = world);

            yield return world;
        }
    }
}