using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NtakliveBackupMod.Scripts.DI;
using NtakliveBackupMod.Scripts.Extensions;
using NtakliveBackupMod.Scripts.Services.Abstractions;
using NtakliveBackupMod.Scripts.Services.Abstractions.Enum;
using NtakliveBackupMod.Scripts.Services.Abstractions.Models;

namespace NtakliveBackupMod.Scripts;

[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
public class ModApi : IModApi
{
    public void InitMod(Mod _modInstance)
    {
        IServiceProvider provider = Bootstrapper.Initialize();

        ModEvents.GameStartDone.RegisterHandler(() => StartWatchdogForCurrentWorld(provider));
    }

    private static async void StartWatchdogForCurrentWorld(IServiceProvider provider)
    {
        var worldProvider = provider.GetRequiredService<IWorldService>();
        var configuration = provider.GetRequiredService<Configuration>();
        var backupService = provider.GetRequiredService<IWorldBackupService>();
        var logger = provider.GetRequiredService<ILogger<ModApi>>();

        SaveInfo currentSaveInfo = GetCurrentSaveInfo(provider);
        World currentWorld = worldProvider.GetCurrentWorld();
        TimeSpan delay = TimeSpan.FromSeconds(configuration.AutoBackupDelay);

        if (configuration.BackupOnWorldLoaded)
        {
            backupService.Backup(currentSaveInfo, BackupMode.BackupOnly);
            logger.Debug("Initial backup has completed successfully.");
        }

        var backupWatchdog = provider.GetRequiredService<IBackupWatchdog>();
        
        using Task watchdogTask = backupWatchdog.Start(currentWorld, currentSaveInfo, delay, BackupMode.SaveAllAndBackup);
        await watchdogTask;
        if (watchdogTask.Exception != null)
        {   
            logger.Exception(watchdogTask.Exception);
        }
    }

    private static SaveInfo GetCurrentSaveInfo(IServiceProvider provider)
    {
        var worldProvider = provider.GetRequiredService<IWorldService>();
        var directoryService = provider.GetRequiredService<IDirectoryService>();

        string saveFolderPath = worldProvider.GetCurrentWorldSaveDirectory();
        string worldFolderPath = directoryService.GetParentDirectoryPath(saveFolderPath);
        string saveName = directoryService.GetDirectoryName(saveFolderPath);
        string worldName = directoryService.GetDirectoryName(worldFolderPath);

        var saveInfo = new SaveInfo
        {
            SaveFolderPath = saveFolderPath,
            WorldFolderPath = worldFolderPath,
            SaveName = saveName,
            WorldName = worldName,
        };

        return saveInfo;
    }
}