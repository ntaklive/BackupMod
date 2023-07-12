using System;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Utils;

namespace BackupMod.Services;

public sealed class Resources : IResources
{
    private readonly ModConfiguration _configuration;
    private readonly IFilesystem _filesystem;

    public Resources(
        ModConfiguration configuration,
        IFilesystem filesystem)
    {
        _configuration = configuration;
        _filesystem = filesystem;
    }

    public string GetBackupsDirectoryPath() =>
        PathHelper.FixFolderPathSeparators(
            string.IsNullOrWhiteSpace(_configuration.General.CustomBackupsFolder)
                ? _filesystem.Path.Combine(GameIO.GetUserGameDataDir(), "Backups")
                : _configuration.General.CustomBackupsFolder
        );

    public string GetArchiveDirectoryPath() =>
        PathHelper.FixFolderPathSeparators(
            string.IsNullOrWhiteSpace(_configuration.Archive.CustomArchiveFolder)
                ? _filesystem.Path.Combine(GameIO.GetUserGameDataDir(), "Archive")
                : _configuration.Archive.CustomArchiveFolder
        );

    public string GetSavesDirectoryPath() => 
        _filesystem.Directory.GetParentDirectoryPath(
            _filesystem.Directory.GetParentDirectoryPath(GameIO.GetSaveGameDir()));

    public string GetWorldsDirectoryPath() => 
        _filesystem.Directory.GetParentDirectoryPath(GameIO.GetWorldDir());
    
    public string GetMd5HashForWorld(string worldName)
    {
        foreach (PathAbstractions.AbstractedLocation location in PathAbstractions.WorldsSearchPaths.GetAvailablePathsList())
        {
            if (location.FullPath.EndsWith(worldName))
            {
                string checksumsTxtFilepath = _filesystem.Path.Combine(location.FullPath, Constants.cFileWorldChecksums);
                if (_filesystem.File.Exists(checksumsTxtFilepath))
                {
                    return Md5HashHelper.ComputeTextHash(checksumsTxtFilepath);
                }
            }
        }

        throw new ArgumentException("It's unable to find checksums.txt", nameof(worldName));
    }
}