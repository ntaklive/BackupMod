#pragma warning disable S1450

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using NtakliveBackupMod.Scripts.Services.Abstractions;
using NtakliveBackupMod.Scripts.Services.Abstractions.Enum;
using NtakliveBackupMod.Scripts.Services.Abstractions.Models;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
public class BackupWatchdog : IBackupWatchdog
{
    private readonly IWorldBackupService _backupService;
    private readonly IChatService _chatService;
    private readonly ILogger<BackupWatchdog> _logger;

    public BackupWatchdog(
        Configuration configuration,
        IWorldBackupService backupService,
        IChatService chatService,
        ILogger<BackupWatchdog> logger)
    {
        if (configuration.EnableChatMessages)
        {
            _chatService = chatService;
        }

        _backupService = backupService;
        _logger = logger;
    }

    public async Task Start(World world, SaveInfo saveInfo, TimeSpan delay, BackupMode backupMode)
    {
        if (delay.TotalSeconds < 10)
        {
            throw new InvalidOperationException("The auto backup delay must be greater than 10 seconds or equals.");
        }

        _logger.Warning("Watchdog have started.");
        
        string firstBackupTime = DateTime.Now.Add(delay).ToShortTimeString();
        _logger.Debug($"The first backup will be at {firstBackupTime}");
        _chatService?.SendMessage($"The first backup will be at {firstBackupTime}");

        var cts = new CancellationTokenSource();
        _ = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(1000);

                if (world == null)
                {
                    _logger.Debug("World is unloaded.");
                    cts.Cancel(false);
                    break;
                }

                if (world.IsRemote())
                {
                    _logger.Debug("Cannot backup remote world.");
                    cts.Cancel(false);
                    break;
                }
            }

            return Task.CompletedTask;
        });

        while (!cts.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(delay.Subtract(TimeSpan.FromSeconds(5)), cts.Token);

                for (var i = 5; i > 0; i--)
                {
                    _logger.Debug($"Backup in {i}...");
                    _chatService?.SendMessage($"Backup in {i}...");

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch (TaskCanceledException)
            {
                break;
            }

            TimeSpan timeSpent;
            string backupFilePath;
            try
            {
                var stopwatch = Stopwatch.StartNew();

                backupFilePath = _backupService.Backup(saveInfo, backupMode);

                stopwatch.Stop();
                timeSpent = stopwatch.Elapsed;
            }
            catch (Exception exception)
            {
                _logger.Error("Something went wrong... Look at the exception message.");
                _logger.Exception(exception);

                _chatService?.SendMessage("Something went wrong... Look at the exception message in console (press F1).");

                break;
            }

            string nextBackupTime = DateTime.Now.Add(delay).ToShortTimeString();

            _logger.Debug("World backup has completed successfully!");
            _logger.Debug($"Time spent: {timeSpent.TotalSeconds:F2} seconds.");
            _logger.Debug($"backup file location: \"{backupFilePath}\".");
            _logger.Debug($"The next backup will be at {nextBackupTime}");

            _chatService?.SendMessage("World backup has completed successfully!");
            _chatService?.SendMessage($"Time spent: {timeSpent.TotalSeconds:F2} seconds.");
            _chatService?.SendMessage($"Backup file location: \"{backupFilePath}\".");
            _chatService?.SendMessage($"The next backup will be at {nextBackupTime}");
        }

        if (!cts.IsCancellationRequested)
        {
            cts.Cancel(false);
        }

        _logger.Warning("Watchdog have terminated.");
    }
}