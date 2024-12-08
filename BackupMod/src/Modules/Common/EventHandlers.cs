using System;
using System.Threading.Tasks;
using BackupMod.Modules.Commands;
using BackupMod.Modules.Commands.Enums;
using BackupMod.Modules.Commands.EventArgs;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BackupMod.Modules.Common;

public sealed partial class CommonModule
{
    private static ModConfiguration _configuration = null!;

    private static async Task GameStartDoneHandlerFuncAsync(IServiceProvider provider)
    {
        _configuration = provider.GetRequiredService<ModConfiguration>();
        
        var autoBackupService = provider.GetRequiredService<IAutoBackupService>();

        _logger.LogDebug("A 'game start done' handler has started");

        _logger.LogTrace("Current save directory path: {Path}", _worldService.GetCurrentSaveDirectoryPath());
        _logger.LogTrace("Current save name: {Name}", _worldService.GetCurrentSaveName());
        _logger.LogTrace("Current world directory path: {Path}", _worldService.GetCurrentWorldDirectoryPath());
        _logger.LogTrace("Current world name: {Name}", _worldService.GetCurrentWorldName());

        _logger.LogTrace("Archive directory path: {Path}", _resources.GetArchiveDirectoryPath());
        _logger.LogTrace("Backups directory path: {Path}", _resources.GetBackupsDirectoryPath());
        _logger.LogTrace("Saves directory path: {Path}", _resources.GetSavesDirectoryPath());
        _logger.LogTrace("Worlds directory path: {Path}", _resources.GetWorldsDirectoryPath());

        void OnConsoleCommandExecuted(object sender, ConsoleCmdEventArgs args)
        {
            if (args.CommandType == ConsoleCmdType.BackupStop)
            {
                if (autoBackupService.IsRunning)
                {
                    autoBackupService.Stop();
                    
                    _logger.LogDebug("The current AutoBackup process has been terminated");
                }
                else
                {
                    _logger.LogDebug("There is no AutoBackup process to stop");
                }
            }

            if (args.CommandType == ConsoleCmdType.BackupStart)
            {
                if (!autoBackupService.IsRunning)
                {
                    autoBackupService.Start();
                    
                    _logger.LogDebug("The AutoBackup process for the current world was manually started");
                }
                else
                {
                    _logger.LogDebug("An AutoBackup process is already started");
                }
            }

            if (_configuration.AutoBackup.ResetDelayTimerAfterManualBackup &&
                args.CommandType == ConsoleCmdType.Backup)
            {
                autoBackupService.ResetDelayTimer();
            }
        }

        ConsoleCmdBase.CommandExecuted += OnConsoleCommandExecuted;

        if (_configuration.Events.BackupOnWorldLoaded)
        {
            (BackupInfo backupInfo, TimeSpan timeElapsed) result = await _backupManager.CreateAsync("Initial backup");
            _logger.LogInformation("The initial backup has completed successfully");
            _logger.LogInformation($"The backup file location: \"{result.backupInfo.Filepath}\"");
        }

        if (_configuration.AutoBackup.Enabled)
        {
            autoBackupService.Start();
        }
    }
}