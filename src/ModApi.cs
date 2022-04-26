using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NtakliveBackupMod.DI;
using NtakliveBackupMod.Services.Abstractions;
using NtakliveBackupMod.Services.Abstractions.Enum;
using NtakliveBackupMod.Services.Abstractions.Models;

namespace NtakliveBackupMod;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ModApi : IModApi
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public void InitMod(Mod _modInstance)
    {
        Bootstrapper.Initialize();

        ModEvents.GameStartDone.RegisterHandler(() => _ = StartWatchdogForCurrentWorld());
    }

    private static async Task StartWatchdogForCurrentWorld()
    {
        var worldService = ServiceLocator.GetRequiredService<IWorldService>();
        var backupService = ServiceLocator.GetRequiredService<IWorldBackupService>();
        var configuration = ServiceLocator.GetRequiredService<Configuration>();
        var logger = ServiceLocator.GetRequiredService<ILogger<ModApi>>();

        SaveInfo currentSaveInfo = worldService.GetCurrentWorldSaveInfo();
        World currentWorld = worldService.GetCurrentWorld();
        TimeSpan delay = TimeSpan.FromSeconds(configuration.AutoBackupDelay);

        if (configuration.BackupOnWorldLoaded)
        {
            backupService.Backup(currentSaveInfo, BackupMode.BackupOnly);
            logger.Debug("Initial backup has completed successfully.");
        }

        var backupWatchdog = ServiceLocator.GetRequiredService<IBackupWatchdog>();
        
        using Task watchdogTask = backupWatchdog.Start(currentWorld, currentSaveInfo, delay, BackupMode.SaveAllAndBackup);
        await watchdogTask;
        if (watchdogTask.Exception != null)
        {   
            logger.Exception(watchdogTask.Exception);
        }
    }
}