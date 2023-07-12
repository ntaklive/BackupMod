using System;
using System.Diagnostics;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Infrastructure
{
    public class GameSaveBackingUpAlgorithm : IGameSaveBackingUpAlgorithm
    {
        private readonly Domain.Game _game;
        private readonly Domain.Mod _mod;

        public GameSaveBackingUpAlgorithm(Domain.Game game, Domain.Mod mod)
        {
            _game = game;
            _mod = mod;
        }
    
        public Result CanBackUpActiveSave()
        {
            if (!_game.IsStarted())
            {
                return Result.Error("The game is not running. There is nothing to back up");
            }
        
            return Result.Success();
        }
    
        public ValueResult<(Backup backup, TimeSpan timeSpent)> BackUpActiveSave(string? title, BackupCaller? caller, bool? saveBeforeBackingUp)
        {
            var stopWatch = Stopwatch.StartNew();
            try
            {
                Result canBackUpResult = CanBackUpActiveSave();
                if (!canBackUpResult.IsSuccess)
                {
                    return ValueResult<(Backup backup, TimeSpan timeSpent)>.Error(canBackUpResult.ErrorMessage);
                }

                GameSave save = _game.ActiveSave!;
                string saveName = save.Name;
                string worldName = save.World.Name;
                string worldHash = save.World.Hash;

                string backupDirectoryPath = _mod.GetBackupDirectoryPath(worldName, worldHash, saveName);
            
                var backupNameWithoutExtension = $"{DateTime.Now:yyyy-dd-M_HH-mm-ss}";
                var backupArchiveFilename = $"{backupNameWithoutExtension}{_mod.Configuration.General.BackupArchiveExtension}";
                var backupManifestFilename = $"{backupNameWithoutExtension}{Domain.Mod.BackupManifestExtension}";
    
                string backupArchiveFilepath = Filesystem.Path.Combine(backupDirectoryPath, backupArchiveFilename);
                string backupManifestFilepath = Filesystem.Path.Combine(backupDirectoryPath, backupManifestFilename);

                GameTime gameTime = GetGameTimeOfActiveSave();
                BackupTime backupTime = BackupTime.Now(gameTime);
                var backupInfo = new BackupInfo(title, caller, backupTime);
                var backupArchive = new BackupArchive(backupArchiveFilepath);
                var backup = new Backup(_game.ActiveSave, backupInfo, backupArchive);
            
                BackupManifest backupManifest = BackupManifest.Empty.Map(backup);
                string backupManifestJson = SimpleJson2.SimpleJson2.SerializeObject(backupManifest);
                Filesystem.File.WriteAllText(backupManifestFilepath, backupManifestJson);
            
                string tempBackupDirectoryPath = Filesystem.Path.Combine(backupDirectoryPath, $"temp_{backupNameWithoutExtension}");
                Filesystem.Directory.Copy(save.DirectoryPath, tempBackupDirectoryPath, true);
                Filesystem.Archive(_mod.Configuration.General.BackupArchiveExtension).CompressDirectory(tempBackupDirectoryPath, backupArchiveFilepath, false);
                Filesystem.Directory.Delete(tempBackupDirectoryPath, true);

                return ValueResult<(Backup backup, TimeSpan timeSpent)>.Success(new ValueTuple<Backup, TimeSpan>(backup, stopWatch.Elapsed));
            }
            catch (Exception exception)
            {
                return ValueResult<(Backup backup, TimeSpan timeSpent)>.Error(exception.Message);
            }
            finally
            {
                stopWatch.Stop();
            }
        }
    
        private GameTime GetGameTimeOfActiveSave()
        {
            ulong worldTime = GameManager.Instance.World.GetWorldTime();
            int day = GameUtils.WorldTimeToDays(worldTime);
            int hour = GameUtils.WorldTimeToHours(worldTime);
            int minute = GameUtils.WorldTimeToMinutes(worldTime);

            return new GameTime(day, hour, minute);
        }
    }
}