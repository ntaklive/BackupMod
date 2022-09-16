using System;
using BackupMod.Manifest;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

public class BackupManifestFactory : IBackupManifestFactory
{
    private readonly IFilesystem _filesystem;
    private readonly IWorldService _worldService;
    private readonly IResources _resources;

    public BackupManifestFactory(
        IFilesystem filesystem,
        IWorldService worldService,
        IResources resources)
    {
        _filesystem = filesystem;
        _worldService = worldService;
        _resources = resources;
    }
    
    public BackupManifest Create(string title, string backupFilename, string worldName, string saveName)
    {
        string md5Hash = _resources.GetMd5HashForWorld(worldName);

        int playersOnlineCount = _worldService.GetPlayersCount();
        
        WorldTime worldTime = _worldService.GetWorldTime();
        DateTime dateTimeNowUtc = DateTime.UtcNow;

        string manifestFilepath = _filesystem.Path.Combine(_resources.GetBackupsDirectoryPath(), worldName, saveName, $"{_filesystem.Path.GetFileNameWithoutExtension(backupFilename)}{ModConfiguration.Constants.BackupManifestExtension}");
        
        var manifest = new BackupManifest
        {
            BackupFilename = backupFilename,
            Filepath = manifestFilepath,
            Title = title,
            Save = new SaveManifestPart
            {
                Name = saveName
            },
            World = new WorldManifestPart
            {
                Name = worldName,
                Md5Hash = md5Hash
            },
            AdditionalInfo = new AdditionalInfoManifestPart
            {
                Online = playersOnlineCount,
                GameTime = new AdditionalInfoManifestPart.GameTimeManifestPart
                {
                    Timestamp = worldTime.Timestamp.ToString(),
                    Day = worldTime.Day,
                    Hour = worldTime.Hour,
                    Minute = worldTime.Minute
                }
            },
            CreationTime = new CreationTimeManifestPart
            {
                Timestamp = dateTimeNowUtc.ToFileTimeUtc().ToString(),
                Formatted = dateTimeNowUtc.ToCultureInvariantString()
            }
        };

        return manifest;
    }
}