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
        _directoryService.CreateDirectory(allBackupsFolderPath);
        
        string worldBackupsFolderPath = _directoryService.GetParentDirectoryPath(saveInfo.BackupsFolderPath);
        _directoryService.CreateDirectory(worldBackupsFolderPath);
        
        string saveBackupsFolderPath = saveInfo.BackupsFolderPath;
        _directoryService.CreateDirectory(saveBackupsFolderPath);
        
        // Archive
        string archiveFolderPath = GetArchiveFolderPath();
        _directoryService.CreateDirectory(archiveFolderPath);
        
        string worldArchiveFolderPath = _directoryService.GetParentDirectoryPath(saveInfo.ArchiveFolderPath);
        _directoryService.CreateDirectory(worldArchiveFolderPath);
        
        string saveArchiveFolderPath = saveInfo.ArchiveFolderPath;
        _directoryService.CreateDirectory(saveArchiveFolderPath);
    }
}