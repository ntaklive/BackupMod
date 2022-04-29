using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

public class WorldBackupService : IWorldBackupService
{
    private readonly Configuration _configuration;
    private readonly IWorldSaverService _saverService;
    private readonly IDirectoryService _directoryService;
    private readonly IPathService _pathService;
    private readonly IArchiveService _archiveService;
    private readonly IFileService _fileService;
    
    private readonly string _archiveExtension;

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

        _archiveExtension = ".zip";
    }

    public string Backup(SaveInfo saveInfo, BackupMode mode)
    {
        if (mode == BackupMode.SaveAllAndBackup)
        {
            _saverService.SaveAll();
        }

        string currentSaveBackupsFolderPath = GetBackupsFolderPath(saveInfo);
        if (!new DirectoryInfo(currentSaveBackupsFolderPath).Exists)
        {
            new DirectoryInfo(currentSaveBackupsFolderPath).Create();
        }
        
        DeleteRedundantBackupFiles(saveInfo);
        DeleteAllTempFolders(saveInfo);

        var backupName = $"{saveInfo.SaveName}_{DateTime.Now:yyyy-dd-M--HH-mm-ss}";

        string tempFolderPath = _pathService.Combine(currentSaveBackupsFolderPath, $"temp_{backupName}");
        _directoryService.Copy(saveInfo.SaveFolderPath, tempFolderPath, true);

        string zipFilepath = _pathService.Combine(currentSaveBackupsFolderPath, $"{backupName}{_archiveExtension}");
        _archiveService.CompressFolderToZip(tempFolderPath, zipFilepath, false);

        _directoryService.Delete(tempFolderPath, true);

        return zipFilepath;
    }

    public Task<string> BackupAsync(SaveInfo saveInfo, BackupMode mode)
    {
        return Task.Factory.StartNew(() => Backup(saveInfo, mode));
    }

    public void Restore(SaveInfo saveInfo, BackupInfo backupInfo)
    {
        _directoryService.Delete(saveInfo.SaveFolderPath, true);
        
        _archiveService.DecompressZipToFolder(backupInfo.Filepath, saveInfo.SaveFolderPath);
    }

    public Task RestoreAsync(SaveInfo saveInfo, BackupInfo backupInfo)
    {
        return Task.Factory.StartNew(() => Restore(saveInfo, backupInfo));
    }

    public BackupInfo[] GetAllBackups(SaveInfo saveInfo)
    {
        var backups = new List<BackupInfo>();

        string backupsFolderPath = GetAllBackupsFolderPath(saveInfo);
        
        string[] backupZipFilePaths = _directoryService.GetFiles(backupsFolderPath, $"{saveInfo.SaveName}_*{_archiveExtension}",
            SearchOption.TopDirectoryOnly);
        
        foreach (string filepath in backupZipFilePaths)
        {
            var fileInfo = new FileInfo(filepath);

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

        return backups.ToArray();
    }

    public void DeleteAllTempFolders(SaveInfo saveInfo)
    {
        string backupsFolderPath = GetAllBackupsFolderPath(saveInfo);
        
        string[] directoriesPaths = _directoryService.GetDirectories(backupsFolderPath);
        
        foreach (string directoryPath in directoriesPaths)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            if (directoryInfo.Name.StartsWith("temp_"))
            {
                _directoryService.Delete(directoryInfo.FullName, true);
            }
        }
    }

    public string GetAllBackupsFolderPath(SaveInfo saveInfo)
    {
        return string.IsNullOrWhiteSpace(_configuration.CustomBackupsFolder)
                ? _pathService.Combine(saveInfo.WorldFolderPath, "Backups")
                : _pathService.Combine(_configuration.CustomBackupsFolder, saveInfo.WorldName);
    }

    public string GetBackupsFolderPath(SaveInfo saveInfo)
    {
        return _pathService.Combine(GetAllBackupsFolderPath(saveInfo), saveInfo.SaveName);
    }

    public void DeleteRedundantBackupFiles(SaveInfo saveInfo)
    {
        while (true)
        {
            BackupInfo[] backups = GetAllBackups(saveInfo);

            if (backups.Length >= _configuration.BackupsLimit)
            {
                string oldestBackupFilePath = backups.OrderBy(info => info.Timestamp.ToFileTimeUtc()).First().Filepath;
                
                _fileService.Delete(oldestBackupFilePath);
            }
            else
            {
                return;
            }
        }
    }
}