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
    
    
    public IEnumerable<WorldInfo> GetWorldsData()
    {
        string allArchivesFolderPath = _gameDirectoriesService.GetArchiveFolderPath();
        string allBackupsFolderPath = _gameDirectoriesService.GetBackupsFolderPath();
        string allSavesFolderPath = _gameDirectoriesService.GetSavesFolderPath();

        string[] allAvailableWorldsPaths =
            _directoryService.GetDirectories(allSavesFolderPath)
                .Concat(_directoryService.GetDirectories(allBackupsFolderPath))
                .Concat(_directoryService.GetDirectories(allArchivesFolderPath))
                .Where(str =>
                    str != allSavesFolderPath &&
                    str != allArchivesFolderPath &&
                    str != allBackupsFolderPath
                )
                .ToArray();

        var allAvailableWorldsInSavesFolder = new List<string>(allAvailableWorldsPaths.Length / 3);
        allAvailableWorldsPaths.ForEach(worldPath =>
        {
            string worldPathInSavesFolder = worldPath
                .Replace(
                    allBackupsFolderPath,
                    allSavesFolderPath
                )
                .Replace(
                    allArchivesFolderPath,
                    allSavesFolderPath
                );
            
            _directoryService.CreateDirectory(worldPathInSavesFolder);

            string[] saves = _directoryService.GetDirectories(worldPath);
            foreach (string savePath in saves)
            {
                string savePathInSavesFolder = savePath
                    .Replace(
                        allBackupsFolderPath,
                        allSavesFolderPath
                    )
                    .Replace(
                        allArchivesFolderPath,
                        allSavesFolderPath
                    );
                
                _directoryService.CreateDirectory(savePathInSavesFolder);

                if (!allAvailableWorldsInSavesFolder.Contains(worldPathInSavesFolder))
                {
                    allAvailableWorldsInSavesFolder.Add(worldPathInSavesFolder);
                }
            }
        });
        
#if DEBUG
        Log.Warning("All available paths:");
        foreach (string path in allAvailableWorldsPaths)
        {
            Log.Warning(path);
        }
#endif

        return GetWorldsFromFolders(allAvailableWorldsInSavesFolder);
    }
    
    private BackupInfo[] GetSaveBackupsFromFolder(string path)
    {
        var backups = new List<BackupInfo>();
        foreach (string filepath in _directoryService.GetFiles(path, $"*{_archiveService.Extension}",
                     SearchOption.TopDirectoryOnly))
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
            _directoryService.CreateDirectory(saveBackupsFolderPath);
            _directoryService.CreateDirectory(saveArchiveFolderPath);

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

    private IEnumerable<WorldInfo> GetWorldsFromFolders(IEnumerable<string> paths)
    {
        var worlds = new List<WorldInfo>();
        foreach (string worldFolderPath in paths)
        {
            string worldName = _directoryService.GetDirectoryName(worldFolderPath);

            string worldBackupsFolderPath = _pathService.Combine(_gameDirectoriesService.GetBackupsFolderPath(), worldName);
            string worldArchiveFolderPath = _pathService.Combine(_gameDirectoriesService.GetArchiveFolderPath(), worldName);
            _directoryService.CreateDirectory(worldBackupsFolderPath);
            _directoryService.CreateDirectory(worldArchiveFolderPath);

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