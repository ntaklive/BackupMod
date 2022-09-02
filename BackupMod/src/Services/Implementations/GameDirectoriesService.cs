using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Utils;

namespace BackupMod.Services;

public sealed class Resources : IResources
{
    private readonly ModConfiguration _configuration;
    private readonly IPathService _pathService;
    private readonly IDirectoryService _directoryService;

    public Resources(
        ModConfiguration configuration,
        IPathService pathService,
        IDirectoryService directoryService)
    {
        _configuration = configuration;
        _pathService = pathService;
        _directoryService = directoryService;
    }

    public string GetBackupsDirectoryPath() =>
        PathHelper.FixFolderPathSeparators(
            string.IsNullOrWhiteSpace(_configuration.General.CustomBackupsFolder)
                ? _pathService.Combine(GameIO.GetUserGameDataDir(), "Backups")
                : _configuration.General.CustomBackupsFolder
        );

    public string GetArchiveDirectoryPath() =>
        PathHelper.FixFolderPathSeparators(
            string.IsNullOrWhiteSpace(_configuration.Archive.CustomArchiveFolder)
                ? _pathService.Combine(GameIO.GetUserGameDataDir(), "Archive")
                : _configuration.Archive.CustomArchiveFolder
        );

    public string GetSavesDirectoryPath() =>
        PathHelper.FixFolderPathSeparators(
            _directoryService.GetParentDirectoryPath(
                _directoryService.GetParentDirectoryPath(GameIO.GetSaveGameDir())));

    public string GetWorldsDirectoryPath() =>
        PathHelper.FixFolderPathSeparators(
            _directoryService.GetParentDirectoryPath(GameIO.GetWorldDir()));
}