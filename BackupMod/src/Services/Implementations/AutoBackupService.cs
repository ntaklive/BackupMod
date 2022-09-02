using System;
using System.Threading;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Game;
using BackupMod.Services.Abstractions.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services;

public class AutoBackupService : IAutoBackupService
{
    private readonly ModConfiguration _configuration;
    private readonly IBackupManager _backupManager;
    private readonly IServerStateWatcher _serverStateWatcher;
    [CanBeNull] private readonly IChatService _chatService;
    private readonly ILogger<AutoBackupService> _logger;

    private readonly TimeSpan _delay;

    private ServerState _currentServerState;

    public AutoBackupService(
        ModConfiguration configuration,
        IBackupManager backupManager,
        IServerStateWatcher serverStateWatcher,
        [CanBeNull] IChatService chatService,
        ILogger<AutoBackupService> logger)
    {
        _configuration = configuration;
        _backupManager = backupManager;
        _serverStateWatcher = serverStateWatcher;
        _chatService = chatService;
        _logger = logger;

        _delay = TimeSpan.FromSeconds(_configuration.AutoBackup.Delay);
        if (_configuration.Notifications.Countdown.Enabled)
        {
            _delay = _delay.Subtract(TimeSpan.FromSeconds(_configuration.Notifications.Countdown.CountFrom));
        }
    }

    public async Task StartAsync(CancellationToken token)
    {
        var cts = new CancellationTokenSource();
        
        async void OnServerStateWatcherOnStateUpdate(object sender, ServerState state) => await ServerStateWatcherOnStateUpdate(state, cts);

        try
        {
            Task watcherTask = _serverStateWatcher.StartAsync(cts);

            _serverStateWatcher.StateUpdate += OnServerStateWatcherOnStateUpdate;

            _logger.LogInformation("AutoBackup process started");
            while (!cts.IsCancellationRequested)
            {
                if (token.IsCancellationRequested)
                {
                    break;    
                }
                
                await Task.Delay(_delay, cts.Token);

                if (_configuration.Notifications.Countdown.Enabled)
                {
                    await ChatCountdown(cts.Token);
                }

                ServerState serverState = _serverStateWatcher.GetServerState();
                if (_configuration.AutoBackup.SkipIfThereAreNoPlayers &&
                    serverState.FillingState == ServerFillingState.Empty)
                {
                    _logger.LogInformation("There are no players on the server. The current backup has been skipped");
                    LogNextBackupTime();

                    continue;
                }

                await BackupAsync(cts.Token);

                LogNextBackupTime();
            }
            
            if (watcherTask.Status != TaskStatus.RanToCompletion)
            {
                cts.Cancel();
                await watcherTask;
            }
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
        finally
        {
            _serverStateWatcher.StateUpdate -= OnServerStateWatcherOnStateUpdate;
            
            _logger.LogInformation("AutoBackup process terminated");
        }
    }

    private async Task ServerStateWatcherOnStateUpdate(ServerState state, CancellationTokenSource cts)
    {
        try
        {
            if (state.AccessibilityState == ServerAccessibilityState.Inaccessible)
            {
                cts.Cancel();
                return;
            }
            
            if (_configuration.Events.BackupOnServerIsEmpty &&
                _currentServerState.FillingState != ServerFillingState.Empty &&
                state.FillingState == ServerFillingState.Empty)
            {
                _logger.LogInformation("The server is empty. Performing a backup");
                
                await BackupAsync(cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "The event cannot be handled correctly");
        }
        finally
        {
            _currentServerState = state;
        }
    }

    private async Task ChatCountdown(CancellationToken token)
    {
        if (_chatService == null)
        {
            return;
        }
        
        for (int i = _configuration.Notifications.Countdown.CountFrom; i > 0; i--)
        {
            _chatService.SendMessage($"Backup in {i}...");
            await Task.Delay(TimeSpan.FromSeconds(1), token);
        }
    }

    private async Task BackupAsync(CancellationToken token)
    {
        try
        {
            _logger.LogInformation("The world backup is starting...");

            (BackupInfo backupInfo, TimeSpan timeElapsed) result =
                await _backupManager.CreateAsync("Automatic backup", BackupMode.SaveAllAndBackup, token);

            _logger.LogInformation("The world backup has completed successfully!");
            _logger.LogInformation($"Time spent: {result.timeElapsed.TotalSeconds:F2} seconds");
            _logger.LogInformation($"The backup file location: \"{result.backupInfo.Filepath}\"");

            _chatService?.SendMessage("The world backup has completed successfully!");
            _chatService?.SendMessage($"Time spent: {result.timeElapsed.TotalSeconds:F2} seconds");
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
    }

    private void LogNextBackupTime()
    {
        string nextBackupTime = DateTime.Now.Add(_delay).ToShortTimeString();
        _logger.LogInformation($"The next backup will be at {nextBackupTime}");
    }
}