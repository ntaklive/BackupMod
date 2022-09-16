using System.Linq;
using BackupMod.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BackupMod.Modules.Commands;

public partial class ConsoleCmdBackup
{
    private void BackupInfoInternal()
    {
        var configuration = Provider.GetRequiredService<ModConfiguration>();
        var resources = Provider.GetRequiredService<IResources>();

        string assemblyVersion = AssemblyInfo.AssemblyVersion;
        string authors = AssemblyInfo.Authors.Aggregate((s, s1) => $"{s}, {s1}");

        _logger.LogInformation($"BackupMod v{assemblyVersion} by {authors}");
        _logger.LogInformation("Current settings:");
        _logger.LogInformation($"General.BackupsLimit: {configuration.General.BackupsLimit.ToString()}");
        _logger.LogInformation($"General.CustomBackupsFolder: {configuration.General.CustomBackupsFolder}");
        _logger.LogInformation($"General.DebugMode: {configuration.General.DebugMode.ToString()}");
        _logger.LogInformation($"AutoBackup.Enabled: {configuration.AutoBackup.Enabled.ToString()}");
        _logger.LogInformation($"AutoBackup.Delay: {configuration.AutoBackup.Delay.ToString()}");
        _logger.LogInformation($"AutoBackup.ResetDelayTimerAfterManualBackup: {configuration.AutoBackup.ResetDelayTimerAfterManualBackup.ToString()}");
        _logger.LogInformation($"AutoBackup.SkipIfThereAreNoPlayers: {configuration.AutoBackup.SkipIfThereAreNoPlayers.ToString()}");
        _logger.LogInformation($"Archive.Enabled: {configuration.Archive.Enabled.ToString()}");
        _logger.LogInformation($"Archive.BackupsLimit: {configuration.Archive.BackupsLimit.ToString()}");
        _logger.LogInformation($"Archive.CustomArchiveFolder: {configuration.Archive.CustomArchiveFolder}");
        _logger.LogInformation($"Events.BackupOnWorldLoaded: {configuration.Events.BackupOnWorldLoaded.ToString()}");
        _logger.LogInformation($"Events.BackupOnServerIsEmpty: {configuration.Events.BackupOnServerIsEmpty.ToString()}");
        _logger.LogInformation($"Notifications.Enabled: {configuration.Notifications.Enabled.ToString()}");
        _logger.LogInformation($"Notifications.Countdown.Enabled: {configuration.Notifications.Countdown.Enabled.ToString()}");
        _logger.LogInformation($"Notifications.Countdown.CountFrom: {configuration.Notifications.Countdown.CountFrom.ToString()}");
        _logger.LogInformation($"Current saves directory: {resources.GetSavesDirectoryPath()}");
    }
}