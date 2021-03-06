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
    private readonly Configuration _configuration;
    private readonly IWorldService _worldService;
    private readonly IWorldBackupService _backupService;
    private readonly IChatService _chatService;
    private readonly ILogger<BackupWatchdog> _logger;

    public BackupWatchdog(
        Configuration configuration,
        IWorldService worldService,
        IWorldBackupService backupService,
        IChatService chatService,
        ILogger<BackupWatchdog> logger)
    {
        _configuration = configuration;
        _worldService = worldService;
        _backupService = backupService;
        _chatService = chatService;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        World world = _worldService.GetCurrentWorld();

        var cts = new CancellationTokenSource();
        _ = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(1000);

                if (!IsWorldAccessible(world))
                {
                    _logger.Debug("Cannot backup a remote/unloaded world.");
                    cts.Cancel();
                    break;
                }
            }

            return Task.CompletedTask;
        });

        await StartAsyncInternal(cts.Token);
    }
    
    private static bool IsWorldAccessible(World world)
    {
        return world != null && !world.IsRemote();
    }

    private async Task StartAsyncInternal(CancellationToken cancellationToken)
    {
        TimeSpan delay = TimeSpan.FromSeconds(_configuration.AutoBackup.Delay);
        
        _logger.Debug("Watchdog have started.");

        string firstBackupTime = DateTime.Now.Add(delay).ToShortTimeString();
        
        _logger.Debug($"The first backup will be at {firstBackupTime}");
        _chatService?.SendMessage($"The first backup will be at {firstBackupTime}");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(delay.Subtract(TimeSpan.FromSeconds(5)), cancellationToken);

                for (var i = 5; i > 0; i--)
                {
                    _chatService?.SendMessage($"Backup in {i}...");

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                break;
            }

            _logger.Debug("The world backup is starting...");
            _chatService?.SendMessage("The world backup is starting...");

            try
            {
                var stopwatch = Stopwatch.StartNew();

                SaveInfo saveInfo = _worldService.GetCurrentWorldSaveInfo();
                string backupFilePath = await _backupService.BackupAsync(saveInfo, BackupMode.SaveAllAndBackup);

                stopwatch.Stop();
                
                TimeSpan timeSpent = stopwatch.Elapsed;

                string nextBackupTime = DateTime.Now.Add(delay).ToShortTimeString();

                _logger.Debug("The world backup has completed successfully!");
                _logger.Debug($"Time spent: {timeSpent.TotalSeconds:F2} seconds.");
                _logger.Debug($"The backup file location: \"{backupFilePath}\".");
                _logger.Debug($"The next backup will be at {nextBackupTime}");

                _chatService?.SendMessage("The world backup has completed successfully!");
                _chatService?.SendMessage($"Time spent: {timeSpent.TotalSeconds:F2} seconds.");
                _chatService?.SendMessage($"The next backup will be at {nextBackupTime}");
            }
            catch (Exception exception)
            {
                _logger.Exception(exception);

                break;
            }
        }
        
        _logger.Debug("Watchdog have terminated.");
    }
}