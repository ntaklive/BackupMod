using System.Collections.Generic;
using System.Linq;
using BackupMod.Services.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace BackupMod.Modules.Commands;

public partial class ConsoleCmdBackup
{
    private void BackupListInternal()
    {
        LogAvailableBackups();
    }
    
    private void LogAvailableBackups()
    {
        IReadOnlyList<WorldInfo> worlds = _worldInfoService.GetWorldInfos();
        if (!worlds.Any())
        {
            _logger.LogInformation("There are no backups here yet");
            return;
        }

        for (var i = 0; i < worlds.Count; i++)
        {
            WorldInfo world = worlds[i];
            
            string worldName = world.Name;
            _logger.LogInformation($"[{i}]: {worldName}");

            IReadOnlyList<SaveInfo> saves = world.Saves;
            for (var j = 0; j < saves.Count; j++)
            {
                SaveInfo save = saves[j];
                
                string saveName = save.Name;
                _logger.LogInformation($"  [{j}]: {saveName}");

                IReadOnlyList<BackupInfo> backups = save.Backups;
                for (var k = 0; k < backups.Count; k++)
                {
                    BackupInfo backup = backups[k];

                    bool archived = backup.Archived;
                    string title = backup.Title;
                    var online = backup.Additional.Online.ToString();
                    var time = $"{backup.Additional.Time.CreationTime.Timestamp:MM.dd.yyyy HH:mm:ss}";
                    var gameTime = backup.Additional.Time.GameTime.ToString();
                    _logger.LogInformation($"    [{k}]: {title}\t{time} ({gameTime.ToLower()}) online: {online} {(archived ? " [archived]" : string.Empty)}");
                }
            }
        }
    }
}