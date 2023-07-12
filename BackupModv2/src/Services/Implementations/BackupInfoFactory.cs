using System;
using BackupMod.Manifest;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Services.Abstractions.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services;

public class BackupInfoFactory : IBackupInfoFactory
{
    private readonly IFilesystem _filesystem;
    private readonly IResources _resources;
    private readonly ILogger<BackupInfoFactory> _logger;

    public BackupInfoFactory(
        IFilesystem filesystem,
        IResources resources,
        ILogger<BackupInfoFactory> logger)
    {
        _filesystem = filesystem;
        _resources = resources;
        _logger = logger;
    }

    [CanBeNull]
    public BackupInfo CreateFromManifest(BackupManifest manifest)
    {
        string archivesDirectoryPath = _resources.GetArchiveDirectoryPath();
        string savesDirectoryPath = _resources.GetSavesDirectoryPath();
        
        string saveDirectoryPath = _filesystem.Path.Combine(savesDirectoryPath,
            manifest.World.Name,
            manifest.Save.Name);
        
        string backupFilepath = _filesystem.Path.Combine(_filesystem.Path.GetDirectoryName(manifest.Filepath), manifest.BackupFilename);
        if (!_filesystem.File.Exists(backupFilepath))
        {
            _logger.LogWarning("Unable to find the backup file specified in the manifest {ManifestFilepath}",
                manifest.Filepath);
            return null;
        }

        bool archived = manifest.Filepath.Contains(archivesDirectoryPath);

        var backup = new BackupInfo
        {
            Title = manifest.Title,
            Filepath = backupFilepath,
            ManifestFilepath = manifest.Filepath,
            Archived = archived,
            SaveDirectoryPath = saveDirectoryPath,
            Additional = new BackupInfo.AdditionalInfo()
            {
                Online = manifest.AdditionalInfo.Online,
                Time = new BackupInfo.AdditionalInfo.TimeInfo()
                {
                    CreationTime = new BackupInfo.AdditionalInfo.CreationTime()
                    {
                        Timestamp = DateTime.FromFileTimeUtc(long.Parse(manifest.CreationTime.Timestamp)),
                        Formatted = manifest.CreationTime.Formatted,
                    },
                    GameTime = new BackupInfo.AdditionalInfo.GameTime()
                    {
                        Day = manifest.AdditionalInfo.GameTime.Day,
                        Hour = manifest.AdditionalInfo.GameTime.Hour,
                        Minute = manifest.AdditionalInfo.GameTime.Minute,
                    }
                }
            }
        };

        return backup;
    }
}