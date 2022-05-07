using System.IO;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utils;

namespace BackupMod.Services;

public class GameDirectoriesService : IGameDirectoriesService
{
    private readonly Configuration _configuration;
    private readonly IPathService _pathService;
    private readonly IDirectoryService _directoryService;

    public GameDirectoriesService(
        Configuration configuration,
        IPathService pathService,
        IDirectoryService directoryService)
    {
        _configuration = configuration;
        _pathService = pathService;
        _directoryService = directoryService;
    }

    public string GetBackupsFolderPath() =>
        PathHelper.FixFolderPathSeparators(
            string.IsNullOrWhiteSpace(_configuration.General.CustomBackupsFolder)
                ? _pathService.Combine(GameIO.GetUserGameDataDir(), "Backups")
                : _configuration.General.CustomBackupsFolder
        );
    
    public string GetArchiveFolderPath() =>
        PathHelper.FixFolderPathSeparators(
            string.IsNullOrWhiteSpace(_configuration.Archive.CustomArchiveFolder)
                ? _pathService.Combine(GameIO.GetUserGameDataDir(), "Archive")
                : _configuration.Archive.CustomArchiveFolder
        );

    public string GetSavesFolderPath() =>
        PathHelper.FixFolderPathSeparators(
            _directoryService.GetParentDirectoryPath(
                _directoryService.GetParentDirectoryPath(GameIO.GetSaveGameDir())));
    
    public void CreateRequiredFolders(SaveInfo saveInfo)
    {
        // Backups
        string allBackupsFolderPath = GetBackupsFolderPath();
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
        
        // Archive
        string archiveFolderPath = GetArchiveFolderPath();
        var archiveFolder = new DirectoryInfo(archiveFolderPath);
        if (!archiveFolder.Exists)
        {
            archiveFolder.Create();
        }
        
        string worldArchiveFolderPath = _directoryService.GetParentDirectoryPath(saveInfo.ArchiveFolderPath);
        var worldArchiveFolder = new DirectoryInfo(worldArchiveFolderPath);
        if (!worldArchiveFolder.Exists)
        {
            worldArchiveFolder.Create();
        }
        
        string saveArchiveFolderPath = saveInfo.ArchiveFolderPath;
        var saveArchiveFolder = new DirectoryInfo(saveArchiveFolderPath);
        if (!saveArchiveFolder.Exists)
        {
            saveArchiveFolder.Create();
        }
    }
}