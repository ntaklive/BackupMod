using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Models;
using BackupMod.Utils;

namespace BackupMod.Services;

public class GameDirectoriesProvider : IGameDirectoriesProvider
{
    private readonly Configuration _configuration;
    private readonly IPathService _pathService;

    public GameDirectoriesProvider(
        Configuration configuration,
        IPathService pathService)
    {
        _configuration = configuration;
        _pathService = pathService;
    }

    public string GetBackupsFolderPath() =>
        PathHelper.FixFolderPathSeparators(
            string.IsNullOrWhiteSpace(_configuration.CustomBackupsFolder)
                ? _pathService.Combine(GameIO.GetUserGameDataDir(), "Backups")
                : _configuration.CustomBackupsFolder
        );

    public string GetSavesFolderPath() =>
        PathHelper.FixFolderPathSeparators(
            _pathService.Combine(GameIO.GetUserGameDataDir(), "Saves")
        );
}