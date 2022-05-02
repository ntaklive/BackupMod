using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utils;

namespace BackupMod.Services;

public class GameDirectoriesProvider : IGameDirectoriesProvider
{
    private readonly Configuration _configuration;
    private readonly IPathService _pathService;
    private readonly IDirectoryService _directoryService;

    public GameDirectoriesProvider(
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
            string.IsNullOrWhiteSpace(_configuration.CustomBackupsFolder)
                ? _pathService.Combine(GameIO.GetUserGameDataDir(), "Backups")
                : _configuration.CustomBackupsFolder
        );

    public string GetSavesFolderPath() =>
        PathHelper.FixFolderPathSeparators(
            _directoryService.GetParentDirectoryPath(
                _directoryService.GetParentDirectoryPath(GameIO.GetSaveGameDir())));
}