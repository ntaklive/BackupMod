using System.Linq;
using Microsoft.Extensions.Logging;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Application.Commands.Args;
using ntaklive.BackupMod.Domain;

namespace ntaklive.BackupMod.Application.Commands
{
    public class BackupInfoCommand : IConsoleCommand<CommandArgs>
    {
        private readonly Game _game;
        private readonly Domain.Mod _mod;
        private readonly ILogger<BackupInfo> _logger;

        public BackupInfoCommand(Game game, Domain.Mod mod, ILogger<BackupInfo> logger)
        {
            _game = game;
            _mod = mod;
            _logger = logger;
        }
    
        public Result CanExecute(CommandArgs args)
        {
            return Result.Success();
        }

        public Result Execute(CommandArgs args)
        {
            Configuration configuration = _mod.Configuration;

            string assemblyVersion = AssemblyInfo.AssemblyVersion;
            string authors = AssemblyInfo.Authors.Aggregate((s, s1) => $"{s}, {s1}");

            _logger.LogInformation($"BackupMod v{assemblyVersion} by {authors}");
        
            _logger.LogInformation("Mod settings:");
            _logger.LogInformation($"General.BackupsLimit: {configuration.General.BackupsLimit.ToString()}");
            _logger.LogInformation($"General.BackupsDirectoryPath: {configuration.General.BackupsDirectoryPath}");
            _logger.LogInformation($"General.DebugMode: {configuration.General.DebugMode.ToString()}");
            _logger.LogInformation($"AutoBackup.Enabled: {configuration.AutoBackup.Enabled.ToString()}");
            _logger.LogInformation($"AutoBackup.Delay: {configuration.AutoBackup.Delay.ToString()}");
            _logger.LogInformation($"AutoBackup.ResetDelayTimerAfterManualBackup: {configuration.AutoBackup.ResetDelayTimerAfterManualBackup.ToString()}");
            _logger.LogInformation($"AutoBackup.SkipIfThereAreNoPlayers: {configuration.AutoBackup.SkipIfThereAreNoPlayers.ToString()}");
            _logger.LogInformation($"Archive.Enabled: {configuration.Archive.Enabled.ToString()}");
            _logger.LogInformation($"Archive.BackupsLimit: {configuration.Archive.BackupsLimit.ToString()}");
            _logger.LogInformation($"Archive.ArchiveDirectoryPath: {configuration.Archive.ArchiveDirectoryPath}");
            _logger.LogInformation($"Events.BackupOnWorldLoaded: {configuration.Events.BackupOnWorldLoaded.ToString()}");
            _logger.LogInformation($"Events.BackupOnServerIsEmpty: {configuration.Events.BackupOnServerIsEmpty.ToString()}");
            _logger.LogInformation($"Notifications.Enabled: {configuration.Notifications.Enabled.ToString()}");
            _logger.LogInformation($"Notifications.Countdown.Enabled: {configuration.Notifications.Countdown.Enabled.ToString()}");
            _logger.LogInformation($"Notifications.Countdown.CountFrom: {configuration.Notifications.Countdown.CountFrom.ToString()}");
        
            _logger.LogInformation("Game settings:");
            _logger.LogInformation($"Current saves directory: {_game.SavesDirectoryPath}");
            _logger.LogInformation($"Current worlds directory: {_game.WorldsDirectoryPath}");

            return Result.Success();
        }
    }
}