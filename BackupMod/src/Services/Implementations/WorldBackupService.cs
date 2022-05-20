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
    private readonly IGameDirectoriesService _directories;
    private readonly IWorldSaverService _saverService;
    private readonly IDirectoryService _directoryService;
    private readonly IPathService _pathService;
    private readonly IArchiveService _archiveService;
    private readonly IFileService _fileService;
    
    public WorldBackupService(
        Configuration configuration,
        IGameDirectoriesService directories,
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

        _directories.CreateRequiredFolders(saveInfo);
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
            _directoryService.DeleteDirectory(backupInfo.SaveInfo.SaveFolderPath, true);
        }

        _archiveService.DecompressToFolder(backupInfo.Filepath, backupInfo.SaveInfo.SaveFolderPath);
    }

    public Task RestoreAsync(BackupInfo backupInfo)
    {
        return Task.Factory.StartNew(() => Restore(backupInfo));
    }

    public void Delete(BackupInfo backupInfo)
    {
        _fileService.Delete(backupInfo.Filepath);
    }

    public Task DeleteAsync(BackupInfo backupInfo)
    {
        return Task.Factory.StartNew(() => Delete(backupInfo));
    }

    private string BackupInternal(SaveInfo saveInfo)
    {
        var backupNameWithExtension = $"{saveInfo.SaveName}_{DateTime.Now:yyyy-dd-M--HH-mm-ss}{_archiveService.Extension}";
        string backupNameWithoutExtension = backupNameWithExtension.Split('.')[0];
        
        // The path of the backup file is identical to the path of the backup folder
        string backupFilePath = _pathService.Combine(saveInfo.BackupsFolderPath, backupNameWithExtension);
        string archiveFilePath = _pathService.Combine(saveInfo.ArchiveFolderPath, backupNameWithExtension);
        string tempFolderPath = _pathService.Combine(saveInfo.BackupsFolderPath, $"temp_{backupNameWithoutExtension}");
        
        _directoryService.CopyDirectory(saveInfo.SaveFolderPath, tempFolderPath, true);
        _archiveService.CompressFolder(tempFolderPath, backupFilePath, false);

        if (_configuration.Archive.Enabled)
        {
            FileInfo thisDateBackupFile = _directoryService
                .GetFiles(saveInfo.ArchiveFolderPath, "*", SearchOption.TopDirectoryOnly)
                .Select(path => new FileInfo(path))
                .FirstOrDefault(file => file.CreationTime.ToShortDateString() == DateTime.Today.ToShortDateString());
            
            if (thisDateBackupFile != null)
            {
                _fileService.Delete(thisDateBackupFile.FullName);
            }
            
            _fileService.Copy(backupFilePath, archiveFilePath, false);
        }
        
        _directoryService.DeleteDirectory(tempFolderPath, true);

        return backupFilePath;
    }

    private void DeleteRedundantBackups(SaveInfo saveInfo)
    {
        List<BackupInfo> backups = saveInfo.Backups.Where(backup => backup.Filepath.StartsWith(_directories.GetBackupsFolderPath())).ToList();
        List<BackupInfo> archiveBackups = saveInfo.Backups.Where(backup => backup.Filepath.StartsWith(_directories.GetArchiveFolderPath())).ToList();
        
        while (true)
        {
            if (backups.Count >= _configuration.General.BackupsLimit)
            {
                BackupInfo oldestBackup = backups.OrderBy(info => info.Timestamp.ToFileTimeUtc()).First();

                _fileService.Delete(oldestBackup.Filepath);

                backups.Remove(oldestBackup);
                
                continue;
            }

            if (_configuration.Archive.Enabled &&
                archiveBackups.Count > _configuration.Archive.BackupsLimit)
            {
                BackupInfo oldestBackup = archiveBackups.OrderBy(info => info.Timestamp.ToFileTimeUtc()).First();

                _fileService.Delete(oldestBackup.Filepath);

                archiveBackups.Remove(oldestBackup);
                
                continue;
            }

            break;
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

            _directoryService.DeleteDirectory(directoryInfo.FullName, true);
        }
    }
}