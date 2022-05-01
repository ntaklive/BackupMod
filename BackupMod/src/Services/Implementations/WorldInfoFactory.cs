using System;
using System.Collections.Generic;
using System.IO;
using BackupMod.Extensions;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utils;

namespace BackupMod.Services;

public class WorldInfoFactory : IWorldInfoFactory
{
    private readonly IDirectoryService _directoryService;
    private readonly IPathService _pathService;
    private readonly IArchiveService _archiveService;
    private readonly IGameDirectoriesProvider _gameDirectoriesProvider;

    public WorldInfoFactory(
        IDirectoryService directoryService,
        IPathService pathService,
        IArchiveService archiveService,
        IGameDirectoriesProvider gameDirectoriesProvider)
    {
        _directoryService = directoryService;
        _pathService = pathService;
        _archiveService = archiveService;
        _gameDirectoriesProvider = gameDirectoriesProvider;
    }

    public WorldInfo CreateFromWorldFolderPath(string worldFolderPath)
    {
        string worldName = _directoryService.GetDirectoryName(worldFolderPath);

        var saves = new List<SaveInfo>();
        foreach (string saveFolderPath in _directoryService.GetDirectories(worldFolderPath))
        {
            string saveName = _directoryService.GetDirectoryName(saveFolderPath);

            string backupsFolderPath = _pathService.Combine(_gameDirectoriesProvider.GetBackupsFolderPath(), worldName, saveName);
            var backupsFolder = new DirectoryInfo(backupsFolderPath);
            if (!backupsFolder.Exists)
            {
                backupsFolder.Create();
            }
            
            var backups = new List<BackupInfo>();

            string[] backupZipFilePaths = _directoryService.GetFiles(backupsFolderPath,
                $"{saveName}_*{_archiveService.Extension}",
                SearchOption.TopDirectoryOnly);

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
                BackupsFolderPath = backupsFolderPath,
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

        return world;
    }
}