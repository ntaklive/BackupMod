using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using BackupMod.Extensions;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utils;

namespace BackupMod.Services;

[SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
public class GameDataProvider : IGameDataProvider
{
    private readonly IArchiveService _archiveService;
    private readonly IPathService _pathService;
    private readonly IDirectoryService _directoryService;
    private readonly IGameDirectoriesService _gameDirectoriesService;

    public GameDataProvider(
        IArchiveService archiveService,
        IPathService pathService,
        IDirectoryService directoryService,
        IGameDirectoriesService gameDirectoriesService)
    {
        _archiveService = archiveService;
        _pathService = pathService;
        _directoryService = directoryService;
        _gameDirectoriesService = gameDirectoriesService;
    }
    
    public WorldInfo[] GetWorldsData()
    {
        string allBackupsFolderPath = _gameDirectoriesService.GetBackupsFolderPath();
        string allSavesFolderPath = _gameDirectoriesService.GetSavesFolderPath();

        string[] allAvailableWorldsPaths =
            Enumerable.Concat(
                    _directoryService.GetDirectories(allBackupsFolderPath),
                    _directoryService.GetDirectories(allSavesFolderPath)
                )
                .Select(str => PathHelper.FixFolderPathSeparators(str)
                    .Replace(
                        _gameDirectoriesService.GetBackupsFolderPath(),
                        _gameDirectoriesService.GetSavesFolderPath()
                    ))
                .Where(str => str != _gameDirectoriesService.GetSavesFolderPath() && str != _gameDirectoriesService.GetArchiveFolderPath())
                .Distinct()
                .ToArray();

#if DEBUG
        Log.Warning("All available paths:");
        foreach (string path in allAvailableWorldsPaths)
        {
            Log.Warning(path);
        }
#endif

        return GetWorldsFromFolders(allAvailableWorldsPaths);
    }

    private static void CreateDirectoryIfRequired(string path)
    {
        var directory = new DirectoryInfo(path);
        if (!directory.Exists)
        {
            directory.Create();
        }
    }

    private BackupInfo[] GetSaveBackupsFromFolder(string path)
    {
        var backups = new List<BackupInfo>();
        foreach (string filepath in _directoryService.GetFiles(path, $"*{_archiveService.Extension}", SearchOption.TopDirectoryOnly))
        {
            var fileInfo = new FileInfo(filepath);

            var backupInfo = new BackupInfo
            {
                Filepath = filepath,
                Timestamp = fileInfo.CreationTime,
                SaveInfo = null
            };

            backups.Add(backupInfo);
        }

        return backups.ToArray();
    }

    private SaveInfo[] GetSavesFromFolder(string path)
    {
        var saves = new List<SaveInfo>();
        foreach (string saveFolderPath in _directoryService.GetDirectories(path))
        {
            string saveName = _directoryService.GetDirectoryName(saveFolderPath);
            string worldName = _directoryService.GetDirectoryName(_directoryService.GetParentDirectoryPath(saveFolderPath));

            string saveBackupsFolderPath = _pathService.Combine(_gameDirectoriesService.GetBackupsFolderPath(), worldName, saveName);
            string saveArchiveFolderPath = _pathService.Combine(_gameDirectoriesService.GetArchiveFolderPath(), worldName, saveName);
            CreateDirectoryIfRequired(saveBackupsFolderPath);
            CreateDirectoryIfRequired(saveArchiveFolderPath);

            var save = new SaveInfo
            {
                SaveName = saveName,
                SaveFolderPath = saveFolderPath,
                BackupsFolderPath = saveBackupsFolderPath,
                ArchiveFolderPath = saveArchiveFolderPath,
                Backups = Enumerable.Concat(
                        GetSaveBackupsFromFolder(saveArchiveFolderPath),
                        GetSaveBackupsFromFolder(saveBackupsFolderPath)
                    )
                    .Distinct()
                    .ToArray(),
                World = null,
            };

            save.Backups.ForEach(info => info.SaveInfo = save);

            saves.Add(save);
        }

        return saves.ToArray();
    }

    public WorldInfo[] GetWorldsFromFolders(string[] paths)
    {
        var worlds = new List<WorldInfo>();
        foreach (string worldFolderPath in paths)
        {
            string worldName = _directoryService.GetDirectoryName(worldFolderPath);

            string worldBackupsFolderPath = _pathService.Combine(_gameDirectoriesService.GetBackupsFolderPath(), worldName);
            string worldArchiveFolderPath = _pathService.Combine(_gameDirectoriesService.GetArchiveFolderPath(), worldName);
            CreateDirectoryIfRequired(worldBackupsFolderPath);
            CreateDirectoryIfRequired(worldArchiveFolderPath);
            
            var world = new WorldInfo
            {
                WorldName = worldName,
                WorldFolderPath = worldFolderPath,
                BackupsFolderPath = worldBackupsFolderPath,
                ArchiveFolderPath = worldArchiveFolderPath,
                Saves = GetSavesFromFolder(worldFolderPath)
            };

            world.Saves.ForEach(info => info.World = world);

            worlds.Add(world);
        }

        return worlds.ToArray();
    }
}