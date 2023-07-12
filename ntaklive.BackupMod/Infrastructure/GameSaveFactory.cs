using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Infrastructure
{
    public class GameSaveFactory : IGameSaveFactory
    {
        private readonly Domain.Game _game;
        private readonly ILogger<GameSaveFactory> _logger;

        public GameSaveFactory(
            Domain.Game game,
            IBackupService backupService,
            ILogger<GameSaveFactory> logger)
        {
            _game = game;
            _backupService = backupService;
            _logger = logger;
        }
    
        public Result<IList<GameSave>> LoadAvailableSaves(GameWorld world)
        {
            try
            {
                var savesList = new List<GameSave>();

                string worldName = world.Name;
                string worldHash = world.Hash;

                string worldSavesDirectoryPath = Filesystem.Path.Combine(_game.SavesDirectoryPath, worldName);
                string[] worldSavesDirectoryPathsArray = Filesystem.Directory.GetDirectories(worldSavesDirectoryPath).ToArray();

                foreach (string worldSaveDirectoryPath in worldSavesDirectoryPathsArray)
                {
                    string saveName = Filesystem.Directory.GetDirectoryName(worldSaveDirectoryPath);
            
                    var save = new GameSave(saveName, worldSaveDirectoryPath, world);

                    Result<IList<Backup>> getSaveBackupsResult = _backupService.GetAvailableBackups(worldName, worldHash, saveName);
                    if (!getSaveBackupsResult.IsSuccess)
                    {
                        _logger.LogError(getSaveBackupsResult.ErrorMessage);
                    }
                    IList<Backup> saveBackups = getSaveBackupsResult.Data ?? new List<Backup>();
            
                    foreach (Backup saveBackup in saveBackups)
                    {
                        Result addBackupResult = save.AddBackup(saveBackup);
                        if (!addBackupResult.IsSuccess)
                        {
                            _logger.LogError(addBackupResult.ErrorMessage);
                        }
                    }
            
                    savesList.Add(save);
                }
            
                return Result<IList<GameSave>>.Success(savesList);
            }
            catch (Exception exception)
            {
                return Result<IList<GameSave>>.Error(exception.Message);
            }
        }
    }
}