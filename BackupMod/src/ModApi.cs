using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BackupMod.DI;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ModApi : IModApi
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "AsyncVoidLambda")]
    public void InitMod(Mod _modInstance)
    {
        Bootstrapper.Initialize();

        ModEvents.GameStartDone.RegisterHandler(async () =>
        {
            try
            {
                await StartWatchdogForCurrentWorld();
            }
            catch (Exception exception)
            {
                var logger = ServiceLocator.GetRequiredService<ILogger<ModApi>>();

                logger.Exception(exception);
            }
        });
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
            await backupService.BackupAsync(currentSaveInfo, BackupMode.BackupOnly);
            logger.Debug("Initial backup has completed successfully.");
        }

        var backupWatchdog = ServiceLocator.GetRequiredService<IBackupWatchdog>();
        
        using Task watchdogTask = backupWatchdog.StartAsync(currentWorld, currentSaveInfo, delay, BackupMode.SaveAllAndBackup);
        await watchdogTask;
        if (watchdogTask.Exception != null)
        {   
            logger.Exception(watchdogTask.Exception);
        }
    }
}