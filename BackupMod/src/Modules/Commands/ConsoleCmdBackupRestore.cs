using System;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace BackupMod.Modules.Commands;

public partial class ConsoleCmdBackup
{
    private async Task BackupRestoreInternal(int? worldId, int? saveId, int? backupId)
    {
        if (_worldService.GetCurrentWorld() != null)
        {
            _logger.LogError("This command can only be executed in the main menu");

            return;
        }
        
        if (worldId == null || saveId == null || backupId == null)
        {
            _logger.LogInformation("Please specify a backup to restore. Hint: 'backup restore *worldId* *saveId* *backupId*'");

            LogAvailableBackups();

            return;
        }

        if (!ValidateArguments(worldId, saveId, backupId))
        {
            return;
        }

        try
        {
            BackupInfo selectedBackup = _worldInfoService.GetWorldInfos()[worldId.Value].Saves[saveId.Value].Backups[backupId.Value];
            
            await _backupManager.RestoreAsync(selectedBackup);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error was occured");
            
            return;
        }
        
        _logger.LogInformation("The backup of the selected save was successfully restored");
    }
}