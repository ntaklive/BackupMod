using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackupMod.Manifest;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Filesystem;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services;

public class BackupManager : IBackupManager
{
    private readonly ModConfiguration _configuration;
    private readonly IBackupManifestService _manifestService;
    private readonly IBackupManifestFactory _manifestFactory;
    private readonly IWorldInfoService _worldInfoService;
    private readonly IBackupInfoFactory _backupInfoFactory;
    private readonly IResources _resources;
    private readonly IWorldService _worldService;
    private readonly IWorldSaveAlgorithm _saveAlgorithm;
    private readonly IFilesystem _filesystem;
    private readonly ILogger<BackupManager> _logger;

    public BackupManager(
        ModConfiguration configuration,
        IBackupManifestService manifestService,
        IBackupManifestFactory manifestFactory,
        IWorldInfoService worldInfoService,
        IBackupInfoFactory backupInfoFactory,
        IResources resources,
        IWorldService worldService,
        IWorldSaveAlgorithm saveAlgorithm,
        IFilesystem filesystem,
        ILogger<BackupManager> logger)
    {
        _configuration = configuration;
        _manifestService = manifestService;
        _manifestFactory = manifestFactory;
        _worldInfoService = worldInfoService;
        _backupInfoFactory = backupInfoFactory;
        _resources = resources;
        _worldService = worldService;
        _saveAlgorithm = saveAlgorithm;
        _filesystem = filesystem;
        _logger = logger;
    }

    public Task<(BackupInfo backupInfo, TimeSpan timeElapsed)> CreateAsync(string title,
        CancellationToken token = default)
    {
        return Task.Factory.StartNew(() => Create(title), token);
    }

    public Task RestoreAsync(BackupInfo backupInfo, CancellationToken token = default)
    {
        return Task.Factory.StartNew(() => Restore(backupInfo), token);
    }

    public Task DeleteAsync(BackupInfo backupInfo, CancellationToken token = default)
    {
        return Task.Factory.StartNew(() => Delete(backupInfo), token);
    }

    private (BackupInfo backupInfo, TimeSpan timeElapsed) Create(string title)
    {
        var stopwatch = Stopwatch.StartNew();

        string worldName = _worldService.GetCurrentWorldName();
        string saveName = _worldService.GetCurrentSaveName();

        CreateRequiredFolders(worldName, saveName);

        BackupInfo backupInfo = CreateInternal(title, worldName, saveName);
        
        DeleteRolledBackups(backupInfo);
        
        stopwatch.Stop();
        return new ValueTuple<BackupInfo, TimeSpan>(backupInfo, stopwatch.Elapsed);
    }

    private void Restore(BackupInfo backupInfo)
    {
        _logger.LogDebug("Restoring a save from the {Path} backup", backupInfo.Filepath);
        _logger.LogTrace("Backup info: {@Info}", backupInfo);
        
        string saveDirectoryPath = backupInfo.SaveDirectoryPath;
        if (_filesystem.Directory.Exists(saveDirectoryPath))
        {
            _logger.LogDebug("The save directory already exists. It will be deleted");
            _filesystem.Directory.Delete(saveDirectoryPath, true);
        }
        
        _filesystem.Archive.DecompressToFolder(backupInfo.Filepath, saveDirectoryPath);
    }

    private void Delete(BackupInfo backupInfo)
    {
        _filesystem.File.Delete(backupInfo.Filepath);
        _filesystem.File.Delete(backupInfo.ManifestFilepath);
    }

    private BackupInfo CreateInternal(string title, string worldName, string saveName)
    {
        string saveDirectoryPath = _worldService.GetCurrentSaveDirectoryPath();

        string backupsDirectoryPath = _resources.GetBackupsDirectoryPath();
        string backupDirectoryPath = _filesystem.Path.Combine(backupsDirectoryPath, worldName, saveName);

        string archivesDirectoryPath = _resources.GetArchiveDirectoryPath();
        string archiveDirectoryPath = _filesystem.Path.Combine(archivesDirectoryPath, worldName, saveName);

        var backupNameWithoutExtension = $"{DateTime.Now:yyyy-dd-M_HH-mm-ss}";
        var backupNameWithExtension = $"{backupNameWithoutExtension}{ModConfiguration.Constants.BackupArchiveExtension}";
        var manifestNameWithExtension = $"{backupNameWithoutExtension}{ModConfiguration.Constants.BackupManifestExtension}";

        string backupFilepath = _filesystem.Path.Combine(backupDirectoryPath, backupNameWithExtension);
        string backupManifestFilepath = _filesystem.Path.Combine(backupDirectoryPath, manifestNameWithExtension);
        string archiveFilepath = _filesystem.Path.Combine(archiveDirectoryPath, backupNameWithExtension);
        string archiveManifestFilepath = _filesystem.Path.Combine(archiveDirectoryPath, manifestNameWithExtension);
        
        BackupManifest manifest = _manifestFactory.Create(title, backupNameWithExtension, worldName, saveName);
        _manifestService.CreateManifest(backupManifestFilepath, manifest);

        string tempFolderPath = _filesystem.Path.Combine(backupDirectoryPath, $"temp_{backupNameWithoutExtension}");
        _filesystem.Directory.Copy(saveDirectoryPath, tempFolderPath, true);
        _filesystem.Archive.CompressFolder(tempFolderPath, backupFilepath, false);
        
        if (_configuration.Archive.Enabled)
        {
            BackupManifest[] manifests = _filesystem.Directory
                .GetFiles(archiveDirectoryPath, $"*{ModConfiguration.Constants.BackupManifestExtension}", SearchOption.TopDirectoryOnly)
                .Select(filepath => _manifestService.ReadManifest(filepath))
                .ToArray();

            BackupInfo currentDateBackup = manifests
                .Select(x => _backupInfoFactory.CreateFromManifest(x))
                .Where(x => x != null)
                .FirstOrDefault(backup => backup.Additional.Time.CreationTime.Timestamp.Date == DateTime.UtcNow.Date);
            
            if (currentDateBackup != null)
            {
                _filesystem.File.Delete(currentDateBackup.ManifestFilepath);
                _filesystem.File.Delete(currentDateBackup.Filepath);
            }

            _filesystem.File.Copy(backupManifestFilepath, archiveManifestFilepath, false);
            _filesystem.File.Copy(backupFilepath, archiveFilepath, false);
        }

        _filesystem.Directory.Delete(tempFolderPath, true);

        return _backupInfoFactory.CreateFromManifest(manifest);
    }

    private void DeleteRolledBackups(BackupInfo backupInfo)
    {
        SaveInfo saveInfo = _worldInfoService.GetWorldInfos()
            .SelectMany(world => world.Saves)
            .FirstOrDefault(save => save.DirectoryPath == backupInfo.SaveDirectoryPath);
        if (saveInfo == null)
        {
            return;
        }
        
        IReadOnlyList<BackupInfo> currentSaveBackups = saveInfo.Backups.Where(x => !x.Archived).ToArray();

        if (!currentSaveBackups.Any())
        {
            return;
        }
        
        _logger.LogTrace("Current save backups: {@CurrentSaveBackups}", currentSaveBackups);

        List<BackupInfo> backups = currentSaveBackups.Where(backup => backup.Archived == false).ToList();
        List<BackupInfo> archivedBackups = currentSaveBackups.Where(backup => backup.Archived == true).ToList();

        while (true)
        {
            if (backups.Count >= _configuration.General.BackupsLimit)
            {
                BackupInfo oldestBackup = backups
                    .OrderBy(info => info.Additional.Time.CreationTime.Timestamp)
                    .First();

                _filesystem.File.Delete(oldestBackup.Filepath);
                _filesystem.File.Delete(oldestBackup.ManifestFilepath);

                backups.Remove(oldestBackup);

                continue;
            }

            if (_configuration.Archive.Enabled && archivedBackups.Count > _configuration.Archive.BackupsLimit)
            {
                BackupInfo oldestBackup = archivedBackups
                    .OrderBy(info => info.Additional.Time.CreationTime.Timestamp)
                    .First();

                _filesystem.File.Delete(oldestBackup.Filepath);
                _filesystem.File.Delete(oldestBackup.ManifestFilepath);

                archivedBackups.Remove(oldestBackup);

                continue;
            }

            break;
        }
    }

    private void CreateRequiredFolders(string worldName, string saveName)
    {
        // Backups
        string backupsDirectoryPath = _resources.GetBackupsDirectoryPath();
        _filesystem.Directory.Create(backupsDirectoryPath);

        string backupWorldDirectoryPath = _filesystem.Path.Combine(backupsDirectoryPath, worldName);
        _filesystem.Directory.Create(backupWorldDirectoryPath);

        string backupSaveDirectoryPath = _filesystem.Path.Combine(backupWorldDirectoryPath, saveName);
        _filesystem.Directory.Create(backupSaveDirectoryPath);

        // Archive
        string archiveDirectoryPath = _resources.GetArchiveDirectoryPath();
        _filesystem.Directory.Create(archiveDirectoryPath);

        string archiveWorldDirectoryPath = _filesystem.Path.Combine(archiveDirectoryPath, worldName);
        _filesystem.Directory.Create(archiveWorldDirectoryPath);

        string archiveSaveDirectoryPath = _filesystem.Path.Combine(archiveWorldDirectoryPath, saveName);
        _filesystem.Directory.Create(archiveSaveDirectoryPath);
    }
}