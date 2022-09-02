using System;
using System.Threading;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BackupMod.Modules.Common;

public sealed partial class CommonModule
{
    private static async Task GameStartDoneHandlerFuncAsync(IServiceProvider provider)
    {
        var autoBackupService = provider.GetRequiredService<IAutoBackupService>();
        var configuration = provider.GetRequiredService<ModConfiguration>();
        var backupManager = provider.GetRequiredService<IBackupManager>();
        var logger = provider.GetRequiredService<ILogger<CommonModule>>();
        
        var worldService = provider.GetRequiredService<IWorldService>();
        var resources = provider.GetRequiredService<IResources>();
        
        logger.LogDebug("A 'game start done' handler has started");
        
        logger.LogTrace("Current save directory path: {Path}", worldService.GetCurrentSaveDirectoryPath());
        logger.LogTrace("Current save name: {Name}", worldService.GetCurrentSaveName());
        logger.LogTrace("Current world directory path: {Path}", worldService.GetCurrentWorldDirectoryPath());
        logger.LogTrace("Current world name: {Name}", worldService.GetCurrentWorldName());
        
        logger.LogTrace("Archive directory path: {Path}", resources.GetArchiveDirectoryPath());
        logger.LogTrace("Backups directory path: {Path}", resources.GetBackupsDirectoryPath());
        logger.LogTrace("Saves directory path: {Path}", resources.GetSavesDirectoryPath());
        logger.LogTrace("Worlds directory path: {Path}", resources.GetWorldsDirectoryPath());
        
        var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        
        try
        {
            if (configuration.Events.BackupOnWorldLoaded)
            {
                (BackupInfo backupInfo, TimeSpan timeElapsed) result = await backupManager.CreateAsync("Initial backup", BackupMode.BackupOnly, token);
                logger.LogInformation("The initial backup has completed successfully");
                logger.LogInformation($"The backup file location: \"{result.backupInfo.Filepath}\"");
            }

            if (configuration.AutoBackup.Enabled)
            {
                await autoBackupService.StartAsync(cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "A critical error was occured");
        }
        finally
        {
            cts.Cancel();
        }
        
        logger.LogDebug("The 'game start done' handler has stopped");
    }
}