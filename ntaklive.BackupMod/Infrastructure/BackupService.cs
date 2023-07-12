using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Infrastructure
{
    public class BackupService : IBackupService
    {
        private readonly Domain.Mod _mod;
        private readonly ILogger<BackupService> _logger;

        public BackupService(Domain.Game game, Domain.Mod mod, ILogger<BackupService> logger)
        {
            _game = game;
            _mod = mod;
            _logger = logger;
        }
    
        public Result<IList<Backup>> GetAvailableBackups(string? worldName, string? worldHash, string? saveName)
        {
            if (worldName == null)
            {
                return Result<IList<Backup>>.Error("The world name cannot be null");
            }
            if (worldHash == null)
            {
                return Result<IList<Backup>>.Error("The world hash cannot be null");
            }
            if (saveName == null)
            {
                return Result<IList<Backup>>.Error("The save name cannot be null");
            }

            try
            {
                string[] backupManifestFilepathArray = Enumerable.Concat(
                        Filesystem.Directory.EnumerateFiles(_mod.GetBackupDirectoryPath(worldName, worldHash, saveName),
                            $"*{Domain.Mod.BackupManifestExtension}",
                            SearchOption.AllDirectories).AsEnumerable(),
                        Filesystem.Directory.EnumerateFiles(_mod.GetArchiveDirectoryPath(worldName, worldHash, saveName),
                            $"*{Domain.Mod.BackupManifestExtension}",
                            SearchOption.AllDirectories).AsEnumerable())
                    .ToArray();

                var backupsList = new List<Backup>();
                foreach (string backupManifestFilepath in backupManifestFilepathArray)
                {
                    Result<Backup> parseBackupResult = ParseBackupFromManifest(backupManifestFilepath);
                    if (!parseBackupResult.IsSuccess)
                    {
                        _logger.LogWarning("Something went wrong during the reading of the manifest: {ErrorMessage} | ({ManifestFilepath})", parseBackupResult.ErrorMessage, backupManifestFilepath);
                        continue;
                    }
                    Backup backup = parseBackupResult.Data!;
                
                    backupsList.Add(backup);
                }
            
                return Result<IList<Backup>>.Success(backupsList);
            }
            catch (Exception exception)
            {
                return Result<IList<Backup>>.Error(exception.Message);
            }
        }
    
        private Result<Backup> ParseBackupFromManifest(string? manifestFilepath)
        {
            try
            {
                if (manifestFilepath == null)
                {
                    return Result<Backup>.Error("The backup manifest filepath cannot be null");
                }
        
                string manifestJson = Filesystem.File.ReadAllText(manifestFilepath);
                var backupDto = SimpleJson2.SimpleJson2.DeserializeObject<BackupManifest>(manifestJson);

                string backupArchiveFilepath = manifestFilepath.Replace(Domain.Mod.BackupManifestExtension, backupDto.ArchiveExtension);
                if (!Filesystem.File.Exists(backupArchiveFilepath))
                {
                    return Result<Backup>.Error("The backup archive doesn't exist");
                }
            
                Result<GameWorld> getWorldResult = _game.GetWorldByNameAndHash(backupDto.WorldName, backupDto.WorldHash);
                if (!getWorldResult.IsSuccess)
                {
                    return Result<Backup>.Error(getWorldResult.ErrorMessage);
                }
                GameWorld world = getWorldResult.Data!;
        
                Result<GameSave> getSaveResult = world.GetSaveByName(backupDto.SaveName);
                if (!getSaveResult.IsSuccess)
                {
                    return Result<Backup>.Error(getSaveResult.ErrorMessage);
                }
                GameSave save = getSaveResult.Data!;

                bool parseBackupCallerResult = Enum.TryParse(backupDto.BackupCaller, out BackupCaller backupCaller);
                if (!parseBackupCallerResult)
                {
                    return Result<Backup>.Error("Invalid backup caller value");
                }
        
                var gameTime = new GameTime(backupDto.GameTimeDay, backupDto.GameTimeHour, backupDto.GameTimeMinute);
                var realTime = new Domain.RealTime(backupDto.RealTimeHour, backupDto.RealTimeMinute, backupDto.RealTimeSecond);
                var realDate = new RealDate(backupDto.RealDateDay, backupDto.RealDateMonth, backupDto.RealDateYear);
                var realDateTime = new RealDateTime(realDate, realTime);
                var backupTime = new BackupTime(gameTime, realDateTime);
        
                var backupInfo = new BackupInfo(backupDto.Title, backupCaller, backupTime);
                var backupArchive = new BackupArchive(backupArchiveFilepath);
        
                var backup = new Backup(save, backupInfo, backupArchive);
            
                return Result<Backup>.Success(backup);
            }
            catch (Exception exception)
            {
                return Result<Backup>.Error(exception.Message);
            }
        }
    }
}