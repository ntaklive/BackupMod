#pragma warning disable S1450

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services;

[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
public class BackupWatchdog : IBackupWatchdog
{
    private readonly IWorldBackupService _backupService;
    private readonly IChatService _chatService;
    private readonly ILogger<BackupWatchdog> _logger;

    public BackupWatchdog(
        IWorldBackupService backupService,
        IChatService chatService,
        ILogger<BackupWatchdog> logger)
    {
        _backupService = backupService;
        _chatService = chatService;
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
                    _chatService?.SendMessage($"Backup in {i}...");

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch (TaskCanceledException)
            {
                break;
            }
            
            _logger.Debug("The world backup is starting...");
            
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
                
                break;
            }

            string nextBackupTime = DateTime.Now.Add(delay).ToShortTimeString();

            _logger.Debug("The world backup has completed successfully!");
            _logger.Debug($"Time spent: {timeSpent.TotalSeconds:F2} seconds.");
            _logger.Debug($"The backup file location: \"{backupFilePath}\".");
            _logger.Debug($"The next backup will be at {nextBackupTime}");

            _chatService?.SendMessage("The world backup has completed successfully!");
            _chatService?.SendMessage($"Time spent: {timeSpent.TotalSeconds:F2} seconds.");
            _chatService?.SendMessage($"The backup file location: \"{backupFilePath}\".");
            _chatService?.SendMessage($"The next backup will be at {nextBackupTime}");
        }

        if (!cts.IsCancellationRequested)
        {
            cts.Cancel(false);
        }

        _logger.Warning("Watchdog have terminated.");
    }
}