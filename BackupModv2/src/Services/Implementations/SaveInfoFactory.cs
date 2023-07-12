using System.Collections.Generic;
using BackupMod.Manifest;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Services.Abstractions.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services;

public class SaveInfoFactory : ISaveInfoFactory
{
    private readonly IFilesystem _filesystem;
    private readonly IResources _resources;
    private readonly IBackupInfoFactory _backupInfoFactory;
    private readonly ILogger<SaveInfoFactory> _logger;

    public SaveInfoFactory(
        IFilesystem filesystem,
        IResources resources,
        IBackupInfoFactory backupInfoFactory,
        ILogger<SaveInfoFactory> logger)
    {
        _filesystem = filesystem;
        _resources = resources;
        _backupInfoFactory = backupInfoFactory;
        _logger = logger;
    }
    
    [CanBeNull]
    public SaveInfo CreateFromManifests(IReadOnlyList<BackupManifest> manifests)
    {
        SaveInfo saveInfo = null;
        var backups = new List<BackupInfo>();
        var firstIteration = true;
        foreach (BackupManifest saveManifest in manifests)
        {
            BackupInfo backupInfo = _backupInfoFactory.CreateFromManifest(saveManifest);
            
            if (backupInfo == null)
            {
                continue;
            }
            
            backups.Add(backupInfo);
            
            if(!firstIteration) continue;

            string saveName = saveManifest.Save.Name;
            string localSaveDirectoryPath = _filesystem.Path.Combine(
                _resources.GetSavesDirectoryPath(),
                saveManifest.World.Name,
                saveName);

            _logger.LogDebug("There are backups of the save that is contained in the {Path} directory", localSaveDirectoryPath);

            saveInfo = new SaveInfo
            {
                Name = saveName,
                DirectoryPath = localSaveDirectoryPath,
                Backups = backups
            };
            
            firstIteration = false;
        }

        return saveInfo;
    }
}