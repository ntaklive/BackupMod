using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace BackupMod.Modules.Commands;

public partial class ConsoleCmdBackup
{
    private async Task BackupDeleteInternal(int? worldId, int? saveId, int? backupId)
    {
        IReadOnlyList<WorldInfo> worlds = _worldInfoService.GetWorldInfos();

        if (worldId == null || saveId == null || backupId == null)
        {
            _logger.LogInformation("Please specify a backup to delete. Hint: 'backup delete *worldId* *saveId* *backupId*'");

            LogAvailableBackups();

            return;
        }

        if (!ValidateArguments(worldId, saveId, backupId))
        {
            return;
        }
        
        try
        {
            await _backupManager.DeleteAsync(worlds[worldId.Value].Saves[saveId.Value].Backups[backupId.Value]);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error was occured");
            
            return;
        }
        
        _logger.LogInformation("The backup of the selected save was successfully deleted");
    }
}