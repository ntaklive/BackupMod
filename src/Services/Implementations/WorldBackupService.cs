using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NtakliveBackupMod.Scripts.Services.Abstractions;
using NtakliveBackupMod.Scripts.Services.Abstractions.Enum;
using NtakliveBackupMod.Scripts.Services.Abstractions.Models;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class WorldBackupService : IWorldBackupService
{
    private readonly Configuration _configuration;
    private readonly IWorldSaverService _saverService;
    private readonly IDirectoryService _directoryService;
    private readonly IPathService _pathService;
    private readonly IArchiveService _archiveService;
    private readonly IFileService _fileService;

    public WorldBackupService(
        Configuration configuration,
        IWorldSaverService saverService,
        IDirectoryService directoryService,
        IPathService pathService,
        IArchiveService archiveService,
        IFileService fileService)
    {
        _configuration = configuration;
        _saverService = saverService;
        _directoryService = directoryService;
        _pathService = pathService;
        _archiveService = archiveService;
        _fileService = fileService;
    }

    public string Backup(SaveInfo saveInfo, BackupMode mode)
    {
        if (mode == BackupMode.SaveAllAndBackup)
        {
            _saverService.SaveAll();
        }

        string backupsFolderPath =
            string.IsNullOrWhiteSpace(_configuration.CustomBackupsFolder)
            ? _pathService.Combine(saveInfo.WorldFolderPath, "Backups")
            : _pathService.Combine(_configuration.CustomBackupsFolder, saveInfo.WorldName);
        if (!new DirectoryInfo(backupsFolderPath).Exists)
        {
            new DirectoryInfo(backupsFolderPath).Create();
        }

        string currentMapBackupsFolderPath = _pathService.Combine(backupsFolderPath, saveInfo.SaveName);
        if (!new DirectoryInfo(currentMapBackupsFolderPath).Exists)
        {
            new DirectoryInfo(currentMapBackupsFolderPath).Create();
        }

        var backups = new List<BackupInfo>();
        foreach (string file in _directoryService.GetFiles(currentMapBackupsFolderPath, $"{saveInfo.SaveName}_*.zip", SearchOption.TopDirectoryOnly))
        {
            var fileInfo = new FileInfo(file);

            DateTime timestamp = fileInfo.LastWriteTimeUtc;

            var backupInfo = new BackupInfo
            {
                Filepath = fileInfo.FullName,
                Timestamp = timestamp,
                SaveName = saveInfo.SaveName,
                WorldName = saveInfo.WorldName
            };

            backups.Add(backupInfo);
        }

        if (backups.Count >= _configuration.BackupsLimit)
        {
            _fileService.Delete(backups.OrderBy(info => info.Timestamp.ToFileTimeUtc()).First().Filepath);
        }

        foreach (string directory in _directoryService.GetDirectories(currentMapBackupsFolderPath))
        {
            var directoryInfo = new DirectoryInfo(directory);

            if (directoryInfo.Name.StartsWith("temp_"))
            {
                _directoryService.Delete(directoryInfo.FullName, true);
            }
        }

        var backupName = $"{saveInfo.SaveName}_{DateTime.Now:yyyy-dd-M--HH-mm-ss}";

        string tempFolderPath = _pathService.Combine(currentMapBackupsFolderPath, $"temp_{backupName}");
        _directoryService.Copy(saveInfo.SaveFolderPath, tempFolderPath, true);

        string zipFilepath = _pathService.Combine(currentMapBackupsFolderPath, $"{backupName}.zip");
        _archiveService.CompressFolderToZip(tempFolderPath, zipFilepath, false);

        _directoryService.Delete(tempFolderPath, true);

        return zipFilepath;
    }
}