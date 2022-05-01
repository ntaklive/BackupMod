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
    private readonly IGameDirectoriesProvider _directories;
    private readonly IWorldSaverService _saverService;
    private readonly IDirectoryService _directoryService;
    private readonly IPathService _pathService;
    private readonly IArchiveService _archiveService;
    private readonly IFileService _fileService;
    
    public WorldBackupService(
        Configuration configuration,
        IGameDirectoriesProvider directories,
        IWorldSaverService saverService,
        IDirectoryService directoryService,
        IPathService pathService,
        IArchiveService archiveService,
        IFileService fileService)
    {
        _configuration = configuration;
        _directories = directories;
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

        CreateRequiredFolders(saveInfo);
        DeleteRedundantBackups(saveInfo);        
        DeleteTempFolders(saveInfo);
        
        return BackupInternal(saveInfo);
    }

    public Task<string> BackupAsync(SaveInfo saveInfo, BackupMode mode)
    {
        return Task.Factory.StartNew(() => Backup(saveInfo, mode));
    }

    public void Restore(BackupInfo backupInfo)
    {
        var saveDirectory = new DirectoryInfo(backupInfo.SaveInfo.SaveFolderPath);
        if (saveDirectory.Exists)
        {
            _directoryService.Delete(backupInfo.SaveInfo.SaveFolderPath, true);
        }

        _archiveService.DecompressToFolder(backupInfo.Filepath, backupInfo.SaveInfo.SaveFolderPath);
    }

    public Task RestoreAsync(BackupInfo backupInfo)
    {
        return Task.Factory.StartNew(() => Restore(backupInfo));
    }

    private string BackupInternal(SaveInfo saveInfo)
    {
        var backupNameWithExtension = $"{saveInfo.SaveName}_{DateTime.Now:yyyy-dd-M--HH-mm-ss}{_archiveService.Extension}";
        string backupNameWithoutExtension = backupNameWithExtension.Split('.')[0];
        
        // The path of the backup file is identical to the path of the backup folder
        string backupFilePath = _pathService.Combine(saveInfo.BackupsFolderPath, backupNameWithExtension);
        string tempFolderPath = _pathService.Combine(saveInfo.BackupsFolderPath, $"temp_{backupNameWithoutExtension}");
        
        _directoryService.Copy(saveInfo.SaveFolderPath, tempFolderPath, true);
        _archiveService.CompressFolder(tempFolderPath, backupFilePath, false);
        _directoryService.Delete(tempFolderPath, true);

        return backupFilePath;
    }

    private void CreateRequiredFolders(SaveInfo saveInfo)
    {
        string allBackupsFolderPath = _directories.GetBackupsFolderPath();
        var allBackupsFolder = new DirectoryInfo(allBackupsFolderPath);
        if (!allBackupsFolder.Exists)
        {
            allBackupsFolder.Create();
        }
        
        string worldBackupsFolderPath = _directoryService.GetParentDirectoryPath(saveInfo.BackupsFolderPath);
        var worldBackupsFolder = new DirectoryInfo(worldBackupsFolderPath);
        if (!worldBackupsFolder.Exists)
        {
            worldBackupsFolder.Create();
        }
        
        string saveBackupsFolderPath = saveInfo.BackupsFolderPath;
        var saveBackupsFolder = new DirectoryInfo(saveBackupsFolderPath);
        if (!saveBackupsFolder.Exists)
        {
            saveBackupsFolder.Create();
        }
    }

    private void DeleteRedundantBackups(SaveInfo saveInfo)
    {
        List<BackupInfo> backups = saveInfo.Backups.ToList();
        while (true)
        {
            if (backups.Count >= _configuration.BackupsLimit)
            {
                BackupInfo oldestBackup = backups.OrderBy(info => info.Timestamp.ToFileTimeUtc()).First();

                _fileService.Delete(oldestBackup.Filepath);

                backups.Remove(oldestBackup);
            }
            else
            {
                break;
            }
        }
    }
    
    private void DeleteTempFolders(SaveInfo saveInfo)
    {
        string backupsFolderPath = saveInfo.BackupsFolderPath;

        if (!new DirectoryInfo(backupsFolderPath).Exists)
        {
            return;
        }
        
        string[] directoriesPaths = _directoryService.GetDirectories(saveInfo.BackupsFolderPath);

        foreach (string directoryPath in directoriesPaths)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            _directoryService.Delete(directoryInfo.FullName, true);
        }
    }
}