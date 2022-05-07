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

        var logger = ServiceLocator.GetRequiredService<ILogger<ModApi>>();
        
        ModEvents.GameStartDone.RegisterHandler(async () =>
        {
            try
            {
                await StartWatchdogForCurrentWorld();
            }
            catch (Exception exception)
            {
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
        
        if (configuration.Events.BackupOnWorldLoaded)
        {
            string backupFilePath = await backupService.BackupAsync(currentSaveInfo, BackupMode.BackupOnly);
            logger.Debug("Initial backup has completed successfully.");
            logger.Debug($"The backup file location: \"{backupFilePath}\".");
        }

        var backupWatchdog = ServiceLocator.GetRequiredService<IBackupWatchdog>();
        
        Task watchdogTask = backupWatchdog.StartAsync();
        await watchdogTask;
        if (watchdogTask.Exception != null)
        {   
            logger.Exception(watchdogTask.Exception);
        }
    }
}