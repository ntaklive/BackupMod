using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace BackupMod.Modules.Commands;

public partial class ConsoleCmdBackup
{
    private async Task BackupRestoreInternal(int? worldId, int? saveId, int? backupId)
    {
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

        IReadOnlyList<WorldInfo> worldInfos = _worldInfoService.GetWorldInfos();
        
        string currentWorldName = _worldService.GetCurrentWorldName();
        string currentSaveName = _worldService.GetCurrentSaveName();
        string selectedWorldName = worldInfos[worldId.Value].Name;
        string selectedSaveName = worldInfos[worldId.Value].Saves[saveId.Value].Name;
        if (currentWorldName == selectedWorldName && currentSaveName == selectedSaveName)
        {
            _logger.LogError("This command cannot be executed on an active world save. Try to specify another parameters");

            return;
        }

        try
        {
            BackupInfo selectedBackup = worldInfos[worldId.Value].Saves[saveId.Value].Backups[backupId.Value];
            
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