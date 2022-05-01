using System.Collections.Generic;
using System.IO;
using System.Linq;
using BackupMod.Extensions;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utils;

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
        string allBackupsFolderPath = _gameDirectoriesProvider.GetBackupsFolderPath();
        string allSavesFolderPath = _gameDirectoriesProvider.GetSavesFolderPath();

        string[] allAvailableWorldsPaths =
            Enumerable.Concat(
                    _directoryService.GetDirectories(allBackupsFolderPath),
                    _directoryService.GetDirectories(allSavesFolderPath)
                )
                .Select(str => PathHelper.FixFolderPathSeparators(str)
                    .Replace(
                        _gameDirectoriesProvider.GetBackupsFolderPath(),
                        _gameDirectoriesProvider.GetSavesFolderPath()
                    ))
                .Distinct()
                .ToArray();

        var worlds = new List<WorldInfo>();
        foreach (string worldFolderPath in allAvailableWorldsPaths)
        {
            string worldName = _directoryService.GetDirectoryName(worldFolderPath);

            string worldBackupsFolderPath =
                _pathService.Combine(_gameDirectoriesProvider.GetBackupsFolderPath(), worldName);
            var worldBackupsFolder = new DirectoryInfo(worldBackupsFolderPath);
            if (!worldBackupsFolder.Exists)
            {
                worldBackupsFolder.Create();
            }

            var saves = new List<SaveInfo>();
            foreach (string saveFolderPath in _directoryService.GetDirectories(worldFolderPath))
            {
                string saveName = _directoryService.GetDirectoryName(saveFolderPath);

                string saveBackupsFolderPath =
                    _pathService.Combine(_gameDirectoriesProvider.GetBackupsFolderPath(), worldName, saveName);
                var saveBackupsFolder = new DirectoryInfo(saveBackupsFolderPath);
                if (!saveBackupsFolder.Exists)
                {
                    saveBackupsFolder.Create();
                }

                var backups = new List<BackupInfo>();

                string[] backupZipFilePaths = _directoryService.GetFiles(saveBackupsFolderPath,
                    $"{saveName}_*{_archiveService.Extension}", SearchOption.TopDirectoryOnly);
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

            worlds.Add(world);
        }

        return worlds;
    }
}